using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Latinx storytelling.
    /// Configured with familismo honorifics (Abuela, Compadre, Don/Doña),
    /// Spanglish code-switching, and communal/community-first weighting.
    ///
    /// NOTE: Latinx encompasses dozens of distinct national and regional cultures.
    /// This template reflects broadly shared patterns and should be customized
    /// for your specific community.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Latinx Context")]
    public class LatinxContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "latinx";
            displayName = "Latinx";
            description = "Starting template for Latinx storytelling. Familismo honorifics, Spanglish code-switching, " +
                          "community-first weighting. Customize for your specific culture and region.";

            collectivityWeight = 0.82f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics (familismo-centered) ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "grandmother", form = "Abuela", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "grandfather", form = "Abuelo", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "respected_man", form = "Don", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "respected_woman", form = "Doña", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "godfather", form = "Compadre", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "godmother", form = "Comadre", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "family_friend", form = "Tío", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "family_friend_female", form = "Tía", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "peer", form = "mija/mijo", tier = RelationshipTier.Acquaintance });

            // --- Spanglish code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "you understand?", culturalForm = "¿entiendes?", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my heart", culturalForm = "mi corazón", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "family", culturalForm = "familia", wholeWordOnly = true });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "God willing", culturalForm = "si Dios quiere", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "yes of course", culturalForm = "sí, claro", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my love", culturalForm = "mi amor", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "cuento",
                displayName = "Cuento (Story)",
                authorGuidance = "A traditional oral story often with a moral. Cuentos connect generations and are told by elders. They often have a cyclical structure.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "dicho",
                displayName = "Dicho (Proverb)",
                authorGuidance = "A well-known proverb or saying that characters invoke to communicate something in shorthand. Dichos carry cultural weight beyond their literal meaning.",
                textPrefix = "\"",
                textSuffix = "\""
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "familismo_moment",
                displayName = "Familismo Moment",
                authorGuidance = "A beat where the needs of the family or community directly supersede individual desire. The tension between self and familia is the drama.",
                requiresResponse = true,
                suggestedResponses = { "La familia primero.", "I understand.", "I won't forget where I come from." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "consejos",
                displayName = "Consejos (Advice)",
                authorGuidance = "Unsolicited but well-meaning advice from an elder or family member. The player must navigate accepting or resisting it with dignity.",
                requiresResponse = false
            });
        }
    }
}
