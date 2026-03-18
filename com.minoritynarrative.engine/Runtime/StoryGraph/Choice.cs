using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// A single selectable option branching from a StoryNode.
    /// </summary>
    [Serializable]
    public class Choice
    {
        /// <summary>Text shown to the player on the choice button.</summary>
        public string text;

        /// <summary>ID of the StoryNode to navigate to when this choice is selected.</summary>
        public string targetNodeId;

        /// <summary>
        /// Short description of how this choice affects the community.
        /// Shown to the player alongside the choice text if the cultural context
        /// has collectivity weighting enabled.
        /// Example: "Leaves Freedmen's Town without protection"
        /// </summary>
        public string communityImpact;

        /// <summary>
        /// Short description of how this choice affects the individual protagonist.
        /// Example: "Moves Isaiah closer to revenge"
        /// </summary>
        public string individualImpact;

        /// <summary>
        /// Collectivity score delta for this choice (-1.0 to 1.0).
        /// Positive = more collective/community-oriented.
        /// Negative = more individual-oriented.
        /// Used to track the player's cumulative collectivity stance.
        /// </summary>
        public float collectivityDelta = 0f;

        /// <summary>Conditions that must all pass for this choice to be visible.</summary>
        public List<NodeCondition> conditions = new List<NodeCondition>();

        /// <summary>Side-effects fired immediately when this choice is selected.</summary>
        public List<NodeTrigger> triggers = new List<NodeTrigger>();
    }
}
