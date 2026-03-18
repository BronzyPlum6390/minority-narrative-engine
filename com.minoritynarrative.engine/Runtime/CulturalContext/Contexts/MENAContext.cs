using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for MENA (Middle East and North Africa) storytelling traditions.
    /// Draws from broadly shared patterns across Arab, Persian, Amazigh (Berber),
    /// Kurdish, and diaspora communities.
    ///
    /// Pre-configured with family/elder honorifics, Arabic phrase code-switching,
    /// oral poetry and sha'ir traditions, hospitality framing, and strong
    /// family-collective weighting.
    ///
    /// NOTE: MENA is extraordinarily diverse in language, religion, ethnicity, and culture.
    /// This template uses broadly shared patterns. Customize significantly for your
    /// specific country, ethnic community, and religious context.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/MENA Context")]
    public class MENAContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "mena";
            displayName = "MENA";
            description = "Starting template for MENA storytelling. Family honorifics, Arabic code-switching, " +
                          "oral poetry tradition, hospitality framing, family-collective weighting.";

            collectivityWeight = 0.82f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics (Modern Standard Arabic / Levantine base) ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Ammo", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Khalto", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "grandfather", form = "Jiddo", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "grandmother", form = "Teta", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "respected_male", form = "Abu [name]", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "respected_female", form = "Umm [name]", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "religious_elder", form = "Sheikh", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "peer", form = "yalla", tier = RelationshipTier.Acquaintance });

            // --- Arabic code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "God willing", culturalForm = "Inshallah", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "God protect you", culturalForm = "Yislamo", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my heart", culturalForm = "ya qalbi", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "welcome", culturalForm = "ahlan wa sahlan", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I swear", culturalForm = "Wallah", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "Praise God", culturalForm = "Alhamdulillah", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "oral_poetry",
                displayName = "Sha'ir / Oral Poetry",
                authorGuidance = "A character delivers a passage of poetry or elevated speech. In many MENA cultures, poetry is how truth is told when direct speech is impossible — it carries political, emotional, and spiritual weight simultaneously.",
                textPrefix = "—\n",
                textSuffix = "\n—"
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "hospitality_obligation",
                displayName = "Hospitality Obligation (Karam)",
                authorGuidance = "A hospitality moment where the host is culturally obligated to give, and the guest is obligated to receive gracefully. Refusal or forced acceptance both carry meaning.",
                requiresResponse = true,
                suggestedResponses = { "I accept with gratitude.", "You honor me.", "Please, you have given enough." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "exile_memory",
                displayName = "Exile / Ghurba Memory",
                authorGuidance = "A beat of displacement — the pain of being away from home, whether physical or cultural. Ghurba is not merely homesickness; it is a fracture in identity.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "family_honor_reckoning",
                displayName = "Family Honor Reckoning",
                authorGuidance = "A moment where individual action directly threatens or restores family reputation in the community. The stakes extend across generations.",
                requiresResponse = true,
                suggestedResponses = { "I understand what this means for us.", "I carry the family name.", "I will not forget who I come from." }
            });
        }
    }
}
