using System.Collections.Generic;

namespace MinorityNarrativeEngine.OralTradition
{
    /// <summary>
    /// Default built-in processor implementations for the four core oral tradition patterns.
    /// These can be replaced by registering a custom processor with the same Role.
    /// </summary>

    public class DefaultCallAndResponseProcessor : IOralTraditionProcessor
    {
        public string Role => "call_and_response";

        public ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context)
        {
            // Pull response options from the cultural context if available,
            // otherwise fall back to universal affirmations.
            var pattern = context?.GetPattern("call_and_response");
            var options = pattern?.suggestedResponses?.Count > 0
                ? pattern.suggestedResponses
                : new List<string> { "That's right.", "Mm-hmm.", "Say that.", "I hear you." };

            return new ProcessorResult
            {
                RequiresResponse = true,
                ResponseOptions  = options,
                Metadata         = { ["pattern"] = "call_and_response" }
            };
        }
    }

    public class DefaultElderWisdomProcessor : IOralTraditionProcessor
    {
        public string Role => "elder_wisdom";

        public ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context)
        {
            var pattern = context?.GetPattern("elder_wisdom");
            return new ProcessorResult
            {
                TextSuffix = pattern?.textSuffix ?? "",
                Metadata   = { ["pattern"] = "elder_wisdom" }
            };
        }
    }

    public class DefaultSignifyingProcessor : IOralTraditionProcessor
    {
        public string Role => "signifying";

        public ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context)
        {
            // Signifying has no automated text changes — the meaning is in the silence.
            // The metadata lets the UI apply any special styling.
            return new ProcessorResult
            {
                Metadata = { ["pattern"] = "signifying", ["indirect"] = "true" }
            };
        }
    }

    public class DefaultTestimonyProcessor : IOralTraditionProcessor
    {
        public string Role => "testimony";

        public ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context)
        {
            var pattern = context?.GetPattern("testimony");
            var options = pattern?.suggestedResponses?.Count > 0
                ? pattern.suggestedResponses
                : new List<string> { "Tell it.", "We know.", "They won't take that from you." };

            return new ProcessorResult
            {
                RequiresResponse = true,
                ResponseOptions  = options,
                Metadata         = { ["pattern"] = "testimony" }
            };
        }
    }
}
