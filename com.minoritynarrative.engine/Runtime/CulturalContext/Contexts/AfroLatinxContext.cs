using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Afro-Latinx storytelling traditions.
    /// Represents the intersection of African diaspora and Latinx cultures —
    /// drawing from Afro-Cuban, Afro-Brazilian, Afro-Dominican, Afro-Puerto Rican,
    /// and broader Afro-Latinx diaspora communities.
    ///
    /// Pre-configured with dual-heritage honorifics, Spanglish + AAVE code-switching,
    /// Afro-diasporic spiritual tradition patterns, and a high collectivity weighting
    /// that reflects both familismo and African communal values.
    ///
    /// NOTE: Afro-Latinx encompasses enormous diversity across countries, ethnic backgrounds,
    /// and religious traditions. This template is a starting point.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Afro-Latinx Context")]
    public class AfroLatinxContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "afro_latinx";
            displayName = "Afro-Latinx";
            description = "Starting template for Afro-Latinx storytelling. Dual-heritage honorifics, " +
                          "Spanglish + AAVE, Afro-diasporic spiritual patterns, high collectivity.";

            collectivityWeight = 0.85f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics (dual-heritage) ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "grandmother", form = "Abuela", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "grandfather", form = "Abuelo", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Mamí", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Papí", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "community_matriarch", form = "La Madrina", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "spiritual_elder", form = "Santera/Santero", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "close_friend", form = "mano/mana", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "peer", form = "pana", tier = RelationshipTier.Acquaintance });

            // --- Spanglish + AAVE code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "family", culturalForm = "familia", wholeWordOnly = true });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my love", culturalForm = "mi amor", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "God willing", culturalForm = "si Dios quiere", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "you all", culturalForm = "y'all", wholeWordOnly = true });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "for real", culturalForm = "de verdad", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I hear you", culturalForm = "te escucho", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "orisha_moment",
                displayName = "Orisha / Spirit Presence",
                authorGuidance = "A moment where a divine spirit (Orisha, Lwa, or similar) makes themselves present — through a sign, a feeling, or a voice. Rooted in Yoruba, Vodou, Candomblé, and Santería traditions. These are not superstition: they are real in the story world.",
                textPrefix = "[The spirit moves...] "
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "double_consciousness",
                displayName = "Double Consciousness",
                authorGuidance = "A beat of existing in two worlds at once — Black and Latinx, belonging fully to neither in the dominant culture's eyes. The character navigates what W.E.B. Du Bois called 'two-ness.'",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "bomba_call",
                displayName = "Bomba / Communal Call",
                authorGuidance = "A rhythmic communal moment — rooted in Bomba, Cumbia, or similar traditions — where movement and music carry political and emotional meaning. Even in text, the rhythm is present.",
                requiresResponse = true,
                suggestedResponses = { "I hear the drum.", "We are here.", "Asé." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "familismo_reckoning",
                displayName = "Familismo Reckoning",
                authorGuidance = "Familismo meets African communal values — the family's needs are absolute, but so is the responsibility to the broader Black community. When these two obligations conflict, the tension is the story.",
                requiresResponse = true,
                suggestedResponses = { "La familia primero.", "My people need me.", "I carry both." }
            });
        }
    }
}
