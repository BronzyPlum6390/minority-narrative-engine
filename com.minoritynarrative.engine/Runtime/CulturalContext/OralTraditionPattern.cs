using System;
using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Defines an oral storytelling rhythm pattern for a cultural context.
    /// Patterns are referenced in story nodes via culturalTags and processed by the dialogue system.
    /// </summary>
    [Serializable]
    public class OralTraditionPattern
    {
        [Tooltip("Identifier matching a culturalTag on story nodes. E.g., 'call_and_response', 'proverb', 'story_circle'.")]
        public string role;

        [Tooltip("Human-readable name shown in the story builder.")]
        public string displayName;

        [TextArea(2, 3)]
        [Tooltip("How the story builder describes this pattern to non-technical authors.")]
        public string authorGuidance;

        [Tooltip("Prefix text injected before the node's body text. Leave empty for none.")]
        public string textPrefix;

        [Tooltip("Suffix text injected after the node's body text. Leave empty for none.")]
        public string textSuffix;

        [Tooltip("If true, this pattern expects a player-typed or button response before advancing.")]
        public bool requiresResponse;

        [Tooltip("Suggested response prompts shown to the player for call-and-response patterns.")]
        public List<string> suggestedResponses = new List<string>();
    }
}
