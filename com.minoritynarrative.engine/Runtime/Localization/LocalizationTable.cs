using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine.Localization
{
    /// <summary>
    /// A deserialized localization table for a single language.
    /// Loaded from a .lang.json file.
    ///
    /// File format:
    /// {
    ///   "languageCode": "es",
    ///   "languageName": "Español",
    ///   "entries": [
    ///     { "key": "node_id::text",    "value": "Translated text here." },
    ///     { "key": "node_id::choice_0","value": "Primera opción" }
    ///   ]
    /// }
    ///
    /// Keys follow the pattern:
    ///   {nodeId}::text          — the node body text
    ///   {nodeId}::choice_{n}    — the nth choice text (0-indexed)
    ///   {nodeId}::community_{n} — the nth choice communityImpact
    ///   {nodeId}::individual_{n}— the nth choice individualImpact
    /// </summary>
    [Serializable]
    public class LocalizationTable
    {
        /// <summary>BCP-47 language code (e.g. "es", "zh-TW", "ht").</summary>
        public string languageCode;

        /// <summary>Human-readable language name for UI dropdowns.</summary>
        public string languageName;

        /// <summary>All translation entries for this language.</summary>
        public List<LocalizationEntry> entries = new List<LocalizationEntry>();

        // Runtime lookup built after deserialization
        [NonSerialized]
        private Dictionary<string, string> _lookup;

        public void BuildLookup()
        {
            _lookup = new Dictionary<string, string>(entries.Count);
            foreach (var e in entries)
                if (!string.IsNullOrEmpty(e.key))
                    _lookup[e.key] = e.value;
        }

        public bool TryGet(string key, out string value)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(key, out value);
        }

        public string Get(string key, string fallback = null)
        {
            if (_lookup == null) BuildLookup();
            return _lookup.TryGetValue(key, out var v) ? v : fallback ?? key;
        }
    }

    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}
