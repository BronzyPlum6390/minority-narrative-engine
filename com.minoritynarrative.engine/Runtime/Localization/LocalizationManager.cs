using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine.Localization
{
    /// <summary>
    /// Manages the active language and translates story node text at runtime.
    ///
    /// Quickstart:
    ///   1. Create .lang.json files for each language (see LocalizationTable format).
    ///   2. Add them as TextAssets to LocalizationManager in the Inspector.
    ///   3. Call SetLanguage("es") to switch languages at runtime.
    ///   4. The DialogueSystem will call LocalizationManager.Translate() automatically
    ///      if a manager instance exists on NarrativeEngine.
    ///
    /// If no translation is found for a key, the original story text is used as fallback.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Localization Manager")]
    public class LocalizationManager : ScriptableObject
    {
        [Tooltip("All language table TextAssets (.lang.json files). Add one per language.")]
        public List<TextAsset> languageFiles = new List<TextAsset>();

        private Dictionary<string, LocalizationTable> _tables;
        private LocalizationTable _activeTable;

        public string ActiveLanguageCode => _activeTable?.languageCode;
        public string ActiveLanguageName => _activeTable?.languageName;

        // -------------------------------------------------------
        // Initialization
        // -------------------------------------------------------

        public void Initialize()
        {
            _tables = new Dictionary<string, LocalizationTable>();
            foreach (var file in languageFiles)
            {
                if (file == null) continue;
                try
                {
                    var table = JsonUtility.FromJson<LocalizationTable>(file.text);
                    table.BuildLookup();
                    _tables[table.languageCode] = table;
                    Debug.Log($"[MNE] Loaded language: {table.languageName} ({table.languageCode})");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[MNE] Failed to load language file '{file.name}': {ex.Message}");
                }
            }
        }

        // -------------------------------------------------------
        // Language switching
        // -------------------------------------------------------

        /// <summary>
        /// Sets the active language by BCP-47 code (e.g. "es", "zh-TW").
        /// Returns false if the language hasn't been loaded.
        /// </summary>
        public bool SetLanguage(string languageCode)
        {
            if (_tables == null) Initialize();

            if (!_tables.TryGetValue(languageCode, out var table))
            {
                Debug.LogWarning($"[MNE] Language '{languageCode}' not found. Staying with current language.");
                return false;
            }

            _activeTable = table;
            Debug.Log($"[MNE] Language set to: {table.languageName}");
            return true;
        }

        /// <summary>Returns all loaded language codes and names for UI dropdowns.</summary>
        public List<(string code, string name)> GetAvailableLanguages()
        {
            if (_tables == null) Initialize();
            var result = new List<(string, string)>();
            foreach (var t in _tables.Values)
                result.Add((t.languageCode, t.languageName));
            return result;
        }

        // -------------------------------------------------------
        // Translation
        // -------------------------------------------------------

        /// <summary>
        /// Translates the body text of a story node.
        /// Falls back to the original text if no translation exists.
        /// </summary>
        public string TranslateNodeText(string nodeId, string originalText)
        {
            if (_activeTable == null) return originalText;
            return _activeTable.Get($"{nodeId}::text", originalText);
        }

        /// <summary>
        /// Translates a choice's display text.
        /// Falls back to the original text if no translation exists.
        /// </summary>
        public string TranslateChoiceText(string nodeId, int choiceIndex, string originalText)
        {
            if (_activeTable == null) return originalText;
            return _activeTable.Get($"{nodeId}::choice_{choiceIndex}", originalText);
        }

        /// <summary>
        /// Translates a choice's community impact text.
        /// </summary>
        public string TranslateCommunityImpact(string nodeId, int choiceIndex, string originalText)
        {
            if (_activeTable == null) return originalText;
            return _activeTable.Get($"{nodeId}::community_{choiceIndex}", originalText);
        }

        /// <summary>
        /// Translates a choice's individual impact text.
        /// </summary>
        public string TranslateIndividualImpact(string nodeId, int choiceIndex, string originalText)
        {
            if (_activeTable == null) return originalText;
            return _activeTable.Get($"{nodeId}::individual_{choiceIndex}", originalText);
        }

        /// <summary>
        /// Generates a starter localization table JSON string for the given story graph.
        /// All values are pre-filled with the original English text — ready for a translator.
        /// </summary>
        public static string GenerateStarterTable(StoryGraph graph, string languageCode, string languageName)
        {
            var table = new LocalizationTable
            {
                languageCode = languageCode,
                languageName = languageName
            };

            foreach (var node in graph.nodes)
            {
                if (!string.IsNullOrEmpty(node.text))
                    table.entries.Add(new LocalizationEntry
                        { key = $"{node.id}::text", value = node.text });

                for (int i = 0; i < node.choices.Count; i++)
                {
                    var c = node.choices[i];
                    if (!string.IsNullOrEmpty(c.text))
                        table.entries.Add(new LocalizationEntry
                            { key = $"{node.id}::choice_{i}", value = c.text });
                    if (!string.IsNullOrEmpty(c.communityImpact))
                        table.entries.Add(new LocalizationEntry
                            { key = $"{node.id}::community_{i}", value = c.communityImpact });
                    if (!string.IsNullOrEmpty(c.individualImpact))
                        table.entries.Add(new LocalizationEntry
                            { key = $"{node.id}::individual_{i}", value = c.individualImpact });
                }
            }

            return JsonUtility.ToJson(table, prettyPrint: true);
        }
    }
}
