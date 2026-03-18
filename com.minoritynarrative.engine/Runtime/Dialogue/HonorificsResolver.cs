using System.Text.RegularExpressions;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Resolves honorific tokens in dialogue text using the active cultural context and
    /// the relationship data from the current session.
    ///
    /// Token format in story JSON: {honorific.TOKEN}
    /// Example: "Listen to me, {honorific.elder}." → "Listen to me, OG." (Black American context)
    ///          "Listen to me, {honorific.elder}." → "Listen to me, Elder." (Indigenous context)
    /// </summary>
    public static class HonorificsResolver
    {
        private static readonly Regex TokenPattern = new Regex(@"\{honorific\.(\w+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Replaces all {honorific.TOKEN} tokens in text using the context's honorific table.
        /// Uses the relationship tier between speakerId and targetId to select the appropriate form.
        /// </summary>
        public static string Resolve(
            string text,
            CulturalContextBase context,
            StorySession session,
            string speakerId,
            string targetId = null)
        {
            if (context == null || string.IsNullOrEmpty(text)) return text;

            return TokenPattern.Replace(text, match =>
            {
                string token = match.Groups[1].Value;
                RelationshipTier tier = DetermineRelationshipTier(session, speakerId, targetId);
                return context.ResolveHonorific(token, tier);
            });
        }

        private static RelationshipTier DetermineRelationshipTier(
            StorySession session, string charA, string charB)
        {
            if (session == null || string.IsNullOrEmpty(charA) || string.IsNullOrEmpty(charB))
                return RelationshipTier.Neutral;

            float score = session.GetRelationship(charA, charB);

            return score switch
            {
                >= 80f  => RelationshipTier.Family,
                >= 60f  => RelationshipTier.Trusted,
                >= 20f  => RelationshipTier.Neutral,
                >= -20f => RelationshipTier.Acquaintance,
                _       => RelationshipTier.Stranger
            };
        }
    }
}
