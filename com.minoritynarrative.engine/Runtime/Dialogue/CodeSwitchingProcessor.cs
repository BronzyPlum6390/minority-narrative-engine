using System.Text.RegularExpressions;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Applies code-switching substitutions to a text string based on the active cultural context.
    /// Substitutions transform standard/neutral phrasing into culturally specific forms.
    /// Players can toggle code-switching off for accessibility via NarrativeEngine.SetCodeSwitching().
    /// </summary>
    public static class CodeSwitchingProcessor
    {
        /// <summary>
        /// Applies all enabled code-switching rules from the given context to the input text.
        /// Returns the text unchanged if code-switching is disabled on the context.
        /// </summary>
        public static string Process(string text, CulturalContextBase context)
        {
            if (context == null || !context.codeSwitchingEnabled || string.IsNullOrEmpty(text))
                return text;

            string result = text;
            foreach (var rule in context.codeSwitchingRules)
            {
                if (string.IsNullOrEmpty(rule.standard) || string.IsNullOrEmpty(rule.culturalForm))
                    continue;

                if (rule.wholeWordOnly)
                {
                    string pattern = $@"\b{Regex.Escape(rule.standard)}\b";
                    result = Regex.Replace(result, pattern, rule.culturalForm, RegexOptions.IgnoreCase);
                }
                else
                {
                    result = result.Replace(rule.standard, rule.culturalForm,
                        System.StringComparison.OrdinalIgnoreCase);
                }
            }
            return result;
        }
    }
}
