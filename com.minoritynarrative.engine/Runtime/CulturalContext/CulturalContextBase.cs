using System;
using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Abstract ScriptableObject base for all cultural contexts.
    /// Subclass this to define a new cultural context.
    /// Configure via the Cultural Template Wizard (Window > Minority Narrative > Cultural Template Wizard).
    /// </summary>
    public abstract class CulturalContextBase : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique key used in story JSON files. E.g., 'black_american', 'indigenous'.")]
        public string contextKey;

        [Tooltip("Human-readable name shown in the story builder UI.")]
        public string displayName;

        [TextArea(2, 4)]
        [Tooltip("Brief description shown in the Cultural Template Wizard.")]
        public string description;

        // -------------------------------------------------------
        // Honorifics
        // -------------------------------------------------------

        [Header("Honorifics")]
        [Tooltip("Tokens and their resolved forms. Token: 'elder' -> Value: 'OG' or 'Auntie' etc.")]
        public List<HonorificsEntry> honorifics = new List<HonorificsEntry>();

        public string ResolveHonorific(string token, RelationshipTier tier = RelationshipTier.Neutral)
        {
            foreach (var entry in honorifics)
            {
                if (entry.token == token && entry.tier == tier)
                    return entry.form;
            }
            // Fallback: find any entry with matching token
            foreach (var entry in honorifics)
            {
                if (entry.token == token)
                    return entry.form;
            }
            return token;
        }

        // -------------------------------------------------------
        // Code-switching
        // -------------------------------------------------------

        [Header("Code-Switching")]
        [Tooltip("Substitution pairs applied to dialogue text at runtime.")]
        public List<CodeSwitchEntry> codeSwitchingRules = new List<CodeSwitchEntry>();

        [Tooltip("If false, code-switching substitutions are skipped (useful for accessibility toggles).")]
        public bool codeSwitchingEnabled = true;

        // -------------------------------------------------------
        // Collectivity weighting
        // -------------------------------------------------------

        [Header("Collectivity")]
        [Range(0f, 1f)]
        [Tooltip("0 = purely individual-focused culture framing. 1 = purely collective-focused. Most contexts sit between 0.5 and 0.9.")]
        public float collectivityWeight = 0.7f;

        [Tooltip("If true, community impact text is shown alongside every choice.")]
        public bool showCommunityImpact = true;

        // -------------------------------------------------------
        // Oral tradition
        // -------------------------------------------------------

        [Header("Oral Tradition")]
        public List<OralTraditionPattern> oralTraditionPatterns = new List<OralTraditionPattern>();

        /// <summary>
        /// Returns the oral tradition pattern matching the given role, or null.
        /// </summary>
        public OralTraditionPattern GetPattern(string role)
        {
            foreach (var p in oralTraditionPatterns)
                if (p.role == role) return p;
            return null;
        }
    }

    // -------------------------------------------------------
    // Supporting data types
    // -------------------------------------------------------

    [Serializable]
    public class HonorificsEntry
    {
        [Tooltip("Token used in story text: {honorific.elder}")]
        public string token;

        [Tooltip("The resolved form in this cultural context.")]
        public string form;

        [Tooltip("Relationship tier this honorific applies to.")]
        public RelationshipTier tier = RelationshipTier.Neutral;
    }

    [Serializable]
    public class CodeSwitchEntry
    {
        [Tooltip("The standard/neutral form in story JSON.")]
        public string standard;

        [Tooltip("The culturally specific form substituted at runtime.")]
        public string culturalForm;

        [Tooltip("If true, only substitute at word boundaries (prevents partial word matches).")]
        public bool wholeWordOnly = true;
    }

    public enum RelationshipTier
    {
        Stranger = 0,
        Acquaintance = 1,
        Neutral = 2,
        Trusted = 3,
        Family = 4,
        Elder = 5
    }
}
