using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Caribbean storytelling traditions.
    /// Draws from broadly shared Afro-Caribbean, Indo-Caribbean, and Creole patterns
    /// across the anglophone, francophone, and hispanophone Caribbean.
    ///
    /// Pre-configured with Creole code-switching, community/elder honorifics,
    /// duppy/spirit oral tradition, and strong diaspora-collective weighting.
    ///
    /// NOTE: The Caribbean spans 30+ territories with distinct national, linguistic,
    /// and ethnic identities. Customize for your specific island and community.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Caribbean Context")]
    public class CaribbeanContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "caribbean";
            displayName = "Caribbean";
            description = "Starting template for Caribbean storytelling. Creole code-switching, " +
                          "elder/community honorifics, spirit tradition, diaspora collectivity. Customize for your island.";

            collectivityWeight = 0.78f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder", form = "Auntie/Uncle", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Miss", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Mister", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "community_leader", form = "the elders dem", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "family", form = "family", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "close_friend", form = "bredren/sistren", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "peer", form = "yute", tier = RelationshipTier.Acquaintance });

            // --- Creole code-switching (Jamaican Patois base, customize per island) ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "you all", culturalForm = "unu", wholeWordOnly = true });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I am", culturalForm = "Mi a", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "going to", culturalForm = "a go", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "Is that right?", culturalForm = "A true?", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my friend", culturalForm = "mi bredren", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "duppy_presence",
                displayName = "Duppy / Spirit Presence",
                authorGuidance = "A spirit, duppy, or ancestor makes themselves known — through a feeling, a sign, or direct speech. These moments are neither metaphor nor superstition: they are real in the story world.",
                textPrefix = "[A presence...] "
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "sankofa_moment",
                displayName = "Sankofa Moment",
                authorGuidance = "A moment of looking back to move forward. The character must reckon with the past — slavery, colonialism, family rupture — before the next step is possible.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "yard_reasoning",
                displayName = "Yard / Reasoning",
                authorGuidance = "Community reasoning — people gathered to talk, argue, and reach understanding together. Decisions are not made alone.",
                requiresResponse = true,
                suggestedResponses = { "Give thanks.", "I man hear you.", "Seen." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "diaspora_longing",
                displayName = "Diaspora Longing",
                authorGuidance = "A beat of displacement — the character is between places, between homes. Neither here nor there fully belongs to them.",
                requiresResponse = false
            });
        }
    }
}
