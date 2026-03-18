using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Southeast Asian storytelling.
    /// Configured with hierarchical honorifics (Filipino, Thai, Vietnamese defaults),
    /// face-saving indirect speech patterns, and family-collective weighting.
    ///
    /// NOTE: Southeast Asia spans many distinct cultures. This template uses broadly shared
    /// patterns but should be customized for your specific cultural community.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Southeast Asian Context")]
    public class SouthEastAsianContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "southeast_asian";
            displayName = "Southeast Asian";
            description = "Starting template for Southeast Asian storytelling. Hierarchical honorifics, " +
                          "family-collective weighting, face-saving speech. Customize for your specific culture.";

            collectivityWeight = 0.80f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics (multi-culture defaults, customize per nation) ---
            honorifics.Clear();
            // Filipino (Tagalog)
            honorifics.Add(new HonorificsEntry { token = "elder_sibling_male", form = "Kuya", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "elder_sibling_female", form = "Ate", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "grandmother", form = "Lola", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "grandfather", form = "Lolo", tier = RelationshipTier.Elder });
            // Thai
            honorifics.Add(new HonorificsEntry { token = "elder_general", form = "Phi", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "younger_general", form = "Nong", tier = RelationshipTier.Acquaintance });
            // Vietnamese
            honorifics.Add(new HonorificsEntry { token = "uncle_paternal", form = "Chú", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "aunt_maternal", form = "Dì", tier = RelationshipTier.Family });
            // Shared
            honorifics.Add(new HonorificsEntry { token = "respected_elder", form = "Po", tier = RelationshipTier.Elder });

            // --- Face-saving code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "you are wrong", culturalForm = "perhaps there is another way to see it", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I disagree", culturalForm = "I wonder if", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "No.", culturalForm = "Maybe next time.", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "family_council",
                displayName = "Family Council",
                authorGuidance = "Decisions are not made alone — they are discussed as a family or community. The weight of others' opinions is visible in the choice framing.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "elder_teaching",
                displayName = "Elder Teaching",
                authorGuidance = "An elder conveys wisdom indirectly through story, metaphor, or example. The lesson is never stated outright.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "face_moment",
                displayName = "Face Moment",
                authorGuidance = "A situation where a character's honor or family reputation is at stake. Choices here carry social consequence beyond the immediate scene.",
                requiresResponse = true,
                suggestedResponses = { "I understand.", "I will not shame us.", "As you say." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "sacrifice_acknowledgment",
                displayName = "Sacrifice Acknowledgment",
                authorGuidance = "A moment where a character acknowledges the sacrifices made by those before them. Grounds motivation in intergenerational debt and love.",
                requiresResponse = false
            });
        }
    }
}
