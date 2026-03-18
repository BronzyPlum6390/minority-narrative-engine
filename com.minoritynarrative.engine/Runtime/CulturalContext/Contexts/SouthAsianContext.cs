using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for South Asian storytelling traditions.
    /// Draws from broadly shared patterns across the Indian subcontinent
    /// (India, Pakistan, Bangladesh, Sri Lanka, Nepal, diaspora communities).
    ///
    /// Pre-configured with hierarchical kinship honorifics, Hinglish code-switching,
    /// izzat (honor/respect) social framing, and strong joint-family collectivity.
    ///
    /// NOTE: South Asia encompasses hundreds of languages and cultures. This template
    /// defaults to broadly recognized patterns. Customize for your specific region,
    /// language community, religion, and caste context.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/South Asian Context")]
    public class SouthAsianContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "south_asian";
            displayName = "South Asian";
            description = "Starting template for South Asian storytelling. Joint-family honorifics, " +
                          "Hinglish code-switching, izzat framing, intergenerational tension. Customize for your region.";

            collectivityWeight = 0.80f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics (North Indian / Hindi-Urdu base) ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Uncle-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Auntie-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "paternal_grandfather", form = "Dada-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "paternal_grandmother", form = "Dadi-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "maternal_grandfather", form = "Nana-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "maternal_grandmother", form = "Nani-ji", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_sibling_male", form = "Bhai", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "elder_sibling_female", form = "Didi", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "respected_person", form = "ji", tier = RelationshipTier.Neutral });

            // --- Hinglish code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "What do you think?", culturalForm = "Kya lagta hai?", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "It doesn't matter", culturalForm = "Koi baat nahi", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "God willing", culturalForm = "Inshallah", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my heart", culturalForm = "mera dil", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "family honor", culturalForm = "izzat", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "izzat_moment",
                displayName = "Izzat Moment (Honor/Respect)",
                authorGuidance = "A beat where family or community honor is directly at stake. Choices here carry social weight that extends beyond the individual — to the family name, the community's perception, and future generations.",
                requiresResponse = true,
                suggestedResponses = { "I understand what is at stake.", "I will not bring shame.", "This is my choice to make." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "joint_family_council",
                displayName = "Joint Family Council",
                authorGuidance = "Decisions are made collectively in the joint family. Everyone has a voice — the elders' voices carry the most weight. Individual desire must be negotiated through the family.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "diaspora_identity",
                displayName = "Diaspora Identity",
                authorGuidance = "A beat specific to the diaspora experience — caught between the homeland culture and the adopted country. Neither world fully claims the character.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "elder_blessing",
                displayName = "Elder Blessing / Ashirvaad",
                authorGuidance = "An elder formally blesses or withholds blessing from a character's action. This is not merely ceremonial — it carries genuine narrative weight and can gate future choices.",
                requiresResponse = true,
                suggestedResponses = { "I receive your blessing with gratitude.", "I understand.", "I ask for your blessing." }
            });
        }
    }
}
