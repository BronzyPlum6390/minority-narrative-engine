using System;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// A side-effect fired when a node is entered or a choice is selected.
    /// </summary>
    [Serializable]
    public class NodeTrigger
    {
        /// <summary>
        /// The type of side-effect.
        /// Values:
        ///   "set_flag"           — sets a named boolean flag in the session
        ///   "clear_flag"         — clears a named boolean flag
        ///   "adjust_relationship"— adds a delta to a character-to-character relationship score
        ///   "adjust_community"   — adds a delta to a community trust/health score
        ///   "fire_event"         — invokes a named UnityEvent on the NarrativeEngine
        ///   "set_variable"       — sets a named float variable in the session
        /// </summary>
        public string type;

        /// <summary>Flag name, character ID, community ID, variable name, or event name.</summary>
        public string key;

        /// <summary>Numeric delta or value for adjust/set operations.</summary>
        public float value;

        /// <summary>For adjust_relationship, the second character in the pair.</summary>
        public string relatedCharacterId;

        public void Apply(StorySession session)
        {
            switch (type)
            {
                case "set_flag":
                    session.SetFlag(key);
                    break;
                case "clear_flag":
                    session.ClearFlag(key);
                    break;
                case "adjust_relationship":
                    session.AdjustRelationship(key, relatedCharacterId, value);
                    break;
                case "adjust_community":
                    session.AdjustCommunityScore(key, value);
                    break;
                case "set_variable":
                    session.SetVariable(key, value);
                    break;
                case "fire_event":
                    // NarrativeEngine listens for this and routes to registered listeners
                    NarrativeEngine.Instance?.FireNamedEvent(key);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"[MNE] Unknown trigger type: {type}");
                    break;
            }
        }
    }
}
