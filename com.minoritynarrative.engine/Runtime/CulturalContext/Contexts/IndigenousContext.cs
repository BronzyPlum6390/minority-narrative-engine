using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Indigenous storytelling.
    /// Configured with clan/nation honorifics, high collective weighting,
    /// story circle oral tradition patterns, and land-relational language.
    ///
    /// NOTE: Indigenous cultures are highly diverse. This template is a starting point.
    /// Authors should consult community members from their specific nation/culture
    /// when building stories, and override these defaults accordingly.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Indigenous Context")]
    public class IndigenousContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "indigenous";
            displayName = "Indigenous";
            description = "Starting template for Indigenous storytelling. High collectivity, story circle patterns, " +
                          "elder/ancestor honorifics, and land-relational language. Customize for your specific nation/culture.";

            collectivityWeight = 0.90f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder", form = "Elder", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Grandmother", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Grandfather", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "ancestor", form = "the Ancestors", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "community_leader", form = "the Council", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "family", form = "kin", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "land", form = "our Mother", tier = RelationshipTier.Family });

            // --- Land-relational code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "the land", culturalForm = "our Mother", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "owns the land", culturalForm = "lives with the land", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I own", culturalForm = "I am in relationship with", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "story_circle",
                displayName = "Story Circle",
                authorGuidance = "Stories are told in a circle with no defined beginning or end — they continue. Use for multi-generational or cyclical narrative beats.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "ancestor_voice",
                displayName = "Ancestor Voice",
                authorGuidance = "An ancestor or elder spirit speaks through a living character. The words carry the weight of generations.",
                textPrefix = "[The voice of the Ancestors] "
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "land_teaching",
                displayName = "Land Teaching",
                authorGuidance = "A teaching that comes from observing the land, animals, or seasons. Grounds the narrative in place and relationship.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "protocol",
                displayName = "Protocol / Ceremony",
                authorGuidance = "A ceremonial or protocol moment that requires the player to show respect. Choice framing should emphasize relationship over outcome.",
                requiresResponse = true,
                suggestedResponses = { "I listen.", "I am grateful.", "I will remember." }
            });
        }
    }
}
