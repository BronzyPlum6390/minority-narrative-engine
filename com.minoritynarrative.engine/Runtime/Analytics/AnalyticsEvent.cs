using System;

namespace MinorityNarrativeEngine.Analytics
{
    /// <summary>
    /// A single recorded event in a playthrough.
    /// </summary>
    [Serializable]
    public class AnalyticsEvent
    {
        /// <summary>Type of event. See AnalyticsEventType constants.</summary>
        public string type;

        /// <summary>UTC timestamp of when this event occurred.</summary>
        public string timestampUtc;

        /// <summary>Node ID associated with this event (if applicable).</summary>
        public string nodeId;

        /// <summary>Node type (dialogue, narration, choice_prompt, etc.)</summary>
        public string nodeType;

        /// <summary>Speaker ID for dialogue nodes.</summary>
        public string speakerId;

        /// <summary>Cultural tags on the node at the time of the event.</summary>
        public string[] culturalTags;

        /// <summary>Index of the choice selected (for choice events).</summary>
        public int choiceIndex = -1;

        /// <summary>Text of the choice selected (for choice events).</summary>
        public string choiceText;

        /// <summary>Collectivity delta of the selected choice.</summary>
        public float collectivityDelta;

        /// <summary>Session collectivity score at the time of this event.</summary>
        public float collectivityScoreAtEvent;

        /// <summary>
        /// Milliseconds the player spent on this node before advancing or choosing.
        /// Useful for identifying nodes that confuse or deeply engage players.
        /// </summary>
        public long dwellTimeMs;
    }

    public static class AnalyticsEventType
    {
        public const string StoryBegin    = "story_begin";
        public const string NodeEnter     = "node_enter";
        public const string ChoiceSelected = "choice_selected";
        public const string StoryComplete = "story_complete";
    }
}
