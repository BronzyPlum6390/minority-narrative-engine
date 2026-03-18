using System.Collections.Generic;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Payload emitted to UI listeners for each step of dialogue.
    /// The DialogueSystem raises this via NarrativeEngine.OnDialogueEvent.
    /// UI components subscribe to that event and render based on this data.
    /// </summary>
    public class DialogueEvent
    {
        /// <summary>The raw node this event was generated from.</summary>
        public StoryNode sourceNode;

        /// <summary>Character ID of the speaker. Empty for narration.</summary>
        public string speakerId;

        /// <summary>
        /// Display name of the speaker after honorific + code-switching resolution.
        /// Empty for narration nodes.
        /// </summary>
        public string speakerDisplayName;

        /// <summary>
        /// Fully resolved dialogue text: tokens replaced, code-switching applied,
        /// oral tradition prefixes/suffixes applied.
        /// </summary>
        public string resolvedText;

        /// <summary>Choices available to the player. Empty if linear flow.</summary>
        public List<ResolvedChoice> choices = new List<ResolvedChoice>();

        /// <summary>True if the player must wait for input before advancing.</summary>
        public bool awaitingInput;

        /// <summary>Optional portrait ID for UI to look up the speaker's image.</summary>
        public string portraitId;

        /// <summary>Cultural tags from the source node (for UI styling).</summary>
        public List<string> culturalTags;

        /// <summary>Collectivity framing hint: "individual", "collective", "both", "none".</summary>
        public string collectiveFrame;

        /// <summary>True if this node uses a call-and-response oral tradition pattern.</summary>
        public bool isCallAndResponse;

        /// <summary>Suggested response strings for call-and-response nodes.</summary>
        public List<string> suggestedResponses;
    }

    /// <summary>
    /// A choice after condition evaluation and text resolution — ready for UI display.
    /// </summary>
    public class ResolvedChoice
    {
        /// <summary>Index into the source node's choices list.</summary>
        public int sourceIndex;

        /// <summary>Display text after token resolution.</summary>
        public string text;

        /// <summary>Community impact text (null if not applicable).</summary>
        public string communityImpact;

        /// <summary>Individual impact text (null if not applicable).</summary>
        public string individualImpact;

        /// <summary>Collectivity score delta for UI indicator.</summary>
        public float collectivityDelta;

        /// <summary>Reference to the source Choice for trigger application on selection.</summary>
        public Choice sourceChoice;
    }
}
