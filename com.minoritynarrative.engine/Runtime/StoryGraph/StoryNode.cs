using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// A single narrative beat in the story graph.
    /// Each node is one moment: a line of dialogue, a narration, or a scene description.
    /// </summary>
    [Serializable]
    public class StoryNode
    {
        /// <summary>Unique identifier for this node. Used by choices to route to the next beat.</summary>
        public string id;

        /// <summary>
        /// Type of node. Controls how the UI and engine treat this beat.
        /// Values: "dialogue", "narration", "choice_prompt", "community_event", "end"
        /// </summary>
        public string type = "dialogue";

        /// <summary>
        /// Character ID of the speaker. Empty for narration nodes.
        /// Resolved at runtime via CharacterRegistry.
        /// </summary>
        public string speakerId;

        /// <summary>
        /// The text body of this beat. Supports token substitution:
        ///   {name} — character display name
        ///   {honorific.elder} — resolved honorific for elder tier
        ///   {community.freedenstown.name} — community name
        /// Code-switching substitutions are applied automatically based on cultural context.
        /// </summary>
        public string text;

        /// <summary>
        /// Optional override for how long the text lingers before auto-advance (seconds).
        /// -1 means wait for player input.
        /// </summary>
        public float displayDuration = -1f;

        /// <summary>
        /// Cultural tags on this beat. Used for analytics, UI styling, and conditional logic.
        /// Examples: "AAVE", "call_and_response", "elder_wisdom", "communal_decision"
        /// </summary>
        public List<string> culturalTags = new List<string>();

        /// <summary>Choices available to the player from this node. Empty if no branching.</summary>
        public List<Choice> choices = new List<Choice>();

        /// <summary>
        /// If choices is empty, this is the ID of the next node (linear flow).
        /// Ignored if choices is non-empty.
        /// </summary>
        public string nextNodeId;

        /// <summary>Side-effects to fire when this node is entered.</summary>
        public List<NodeTrigger> triggers = new List<NodeTrigger>();

        /// <summary>Conditions that must all pass for this node to be reachable.</summary>
        public List<NodeCondition> entryConditions = new List<NodeCondition>();

        /// <summary>
        /// Framing hint for the collective-vs-individual UI indicator.
        /// Values: "individual", "collective", "both", "none"
        /// </summary>
        public string collectiveFrame = "none";
    }
}
