using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for Black American storytelling.
    /// Pre-configured with AAVE code-switching rules, kinship honorifics,
    /// and oral tradition patterns rooted in call-and-response and elder wisdom traditions.
    ///
    /// This is a starting template — customize via the Cultural Template Wizard
    /// or directly in the Inspector.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/Black American Context")]
    public class BlackAmericanContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "black_american";
            displayName = "Black American";
            description = "Narrative context for Black American storytelling. Includes AAVE language patterns, " +
                          "kinship honorifics, high collectivity weighting, and call-and-response oral tradition.";

            collectivityWeight = 0.75f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder", form = "OG", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Auntie", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_male", form = "Uncle", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "respected_peer", form = "Big Homie", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "community_leader", form = "Big Mama", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "stranger", form = "Folks", tier = RelationshipTier.Stranger });
            honorifics.Add(new HonorificsEntry { token = "family", form = "Blood", tier = RelationshipTier.Family });

            // --- AAVE Code-switching rules ---
            // These are opt-in substitutions. Toggle codeSwitchingEnabled to control.
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "going to", culturalForm = "finna", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "you all", culturalForm = "y'all", wholeWordOnly = true });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I am telling you", culturalForm = "I'm telling you", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "for real", culturalForm = "for real for real", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "He is", culturalForm = "He be", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "She is always", culturalForm = "She be always", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "call_and_response",
                displayName = "Call and Response",
                authorGuidance = "A classic oral tradition beat where the speaker says something and the community (or player) affirms it. Great for communal scenes.",
                requiresResponse = true,
                suggestedResponses = { "That's right.", "Mm-hmm.", "Say that.", "I hear you." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "elder_wisdom",
                displayName = "Elder Wisdom",
                authorGuidance = "An elder character delivers a proverb or hard-earned truth. These moments slow the pace and carry moral weight.",
                textPrefix = "",
                textSuffix = " — remember that."
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "signifying",
                displayName = "Signifying",
                authorGuidance = "Indirect speech with layered meaning. The character says one thing but means another — irony, wit, and coded critique.",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "testimony",
                displayName = "Testimony",
                authorGuidance = "A character witnesses or recounts something true and painful. Community members affirm the truth of what was said.",
                requiresResponse = true,
                suggestedResponses = { "Tell it.", "We know.", "They won't take that from you." }
            });
        }
    }
}
