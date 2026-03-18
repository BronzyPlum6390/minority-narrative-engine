using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Cultural context for West African storytelling traditions.
    /// Pre-configured with elder/community honorifics, ubuntu-relational philosophy,
    /// griot oral tradition patterns, and high collective weighting.
    ///
    /// Draws from broadly shared patterns across West African cultures (Yoruba, Akan,
    /// Wolof, Igbo, Mandé and others). Customize for your specific cultural community.
    ///
    /// NOTE: West Africa spans over 20 countries with hundreds of distinct cultures.
    /// This template is a starting point — consult community members and customize.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Contexts/West African Context")]
    public class WestAfricanContext : CulturalContextBase
    {
        private void Reset()
        {
            contextKey = "west_african";
            displayName = "West African";
            description = "Starting template for West African storytelling. Griot oral tradition, " +
                          "elder/community honorifics, ubuntu philosophy, and high collectivity. Customize for your culture.";

            collectivityWeight = 0.85f;
            showCommunityImpact = true;
            codeSwitchingEnabled = true;

            // --- Honorifics ---
            honorifics.Clear();
            honorifics.Add(new HonorificsEntry { token = "elder", form = "Baba", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "elder_female", form = "Mama", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "chief", form = "Oba", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "griot", form = "Griot", tier = RelationshipTier.Trusted });
            honorifics.Add(new HonorificsEntry { token = "community_leader", form = "the Council of Elders", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "ancestor", form = "the Ancestors", tier = RelationshipTier.Elder });
            honorifics.Add(new HonorificsEntry { token = "family", form = "our blood", tier = RelationshipTier.Family });
            honorifics.Add(new HonorificsEntry { token = "peer", form = "brother/sister", tier = RelationshipTier.Trusted });

            // --- Ubuntu philosophy code-switching ---
            codeSwitchingRules.Clear();
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I am because of myself", culturalForm = "I am because we are", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "my success", culturalForm = "our success", wholeWordOnly = false });
            codeSwitchingRules.Add(new CodeSwitchEntry { standard = "I alone", culturalForm = "we alone", wholeWordOnly = false });

            // --- Oral tradition patterns ---
            oralTraditionPatterns.Clear();
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "griot_telling",
                displayName = "Griot Telling",
                authorGuidance = "The griot is the keeper of history, genealogy, and communal memory. When the griot speaks, the whole community listens. These beats carry generational weight.",
                textPrefix = "[The Griot speaks] ",
                requiresResponse = false
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "ubuntu_moment",
                displayName = "Ubuntu Moment",
                authorGuidance = "A beat where the philosophy 'I am because we are' becomes lived reality. The character's identity and the community's identity are inseparable.",
                requiresResponse = true,
                suggestedResponses = { "We are.", "I hear the community in you.", "Ubuntu." }
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "ancestor_counsel",
                displayName = "Ancestor Counsel",
                authorGuidance = "The ancestors speak through a living character, a dream, or a vision. Their wisdom is not metaphor — it is instruction.",
                textPrefix = "[Voice of the Ancestors] "
            });
            oralTraditionPatterns.Add(new OralTraditionPattern
            {
                role = "proverb",
                displayName = "Proverb",
                authorGuidance = "A traditional proverb that encodes communal wisdom. Characters invoke proverbs to say things that direct speech cannot. Common in Yoruba, Akan, Igbo storytelling.",
                textPrefix = "\"",
                textSuffix = "\""
            });
        }
    }
}
