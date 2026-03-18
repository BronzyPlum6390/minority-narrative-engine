using System;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// A runtime condition evaluated against StorySession state.
    /// All conditions on a node or choice must pass for it to be available.
    /// </summary>
    [Serializable]
    public class NodeCondition
    {
        /// <summary>
        /// The type of check to perform.
        /// Values:
        ///   "flag"         — checks if a named flag is set (or not set)
        ///   "relationship" — checks a relationship score between two characters
        ///   "community"    — checks a community trust/health score
        ///   "collectivity" — checks the player's cumulative collectivity score
        ///   "visited"      — checks if a node has been visited
        /// </summary>
        public string type;

        /// <summary>Name of the flag, character ID, community ID, or node ID depending on type.</summary>
        public string key;

        /// <summary>
        /// Comparison operator. Values: "eq", "neq", "gt", "gte", "lt", "lte", "set", "unset"
        /// "set" / "unset" are for flag type only.
        /// </summary>
        public string op;

        /// <summary>Value to compare against (for numeric comparisons). Ignored for "set"/"unset".</summary>
        public float value;

        /// <summary>
        /// For relationship conditions, the second character in the relationship.
        /// key = character A ID, relatedCharacterId = character B ID.
        /// </summary>
        public string relatedCharacterId;

        public bool Evaluate(StorySession session)
        {
            switch (type)
            {
                case "flag":
                    bool hasFlag = session.HasFlag(key);
                    return op == "set" ? hasFlag : !hasFlag;

                case "relationship":
                    float rel = session.GetRelationship(key, relatedCharacterId);
                    return CompareFloat(rel);

                case "community":
                    float trust = session.GetCommunityScore(key);
                    return CompareFloat(trust);

                case "collectivity":
                    return CompareFloat(session.collectivityScore);

                case "visited":
                    bool visited = session.HasVisited(key);
                    return op == "set" ? visited : !visited;

                default:
                    UnityEngine.Debug.LogWarning($"[MNE] Unknown condition type: {type}");
                    return true;
            }
        }

        private bool CompareFloat(float subject)
        {
            return op switch
            {
                "eq"  => MathF.Abs(subject - value) < 0.001f,
                "neq" => MathF.Abs(subject - value) >= 0.001f,
                "gt"  => subject > value,
                "gte" => subject >= value,
                "lt"  => subject < value,
                "lte" => subject <= value,
                _     => true
            };
        }
    }
}
