using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine.OralTradition
{
    /// <summary>
    /// Global registry for IOralTraditionProcessor implementations.
    /// The DialogueSystem checks this registry before falling back to the
    /// CulturalContextBase's built-in OralTraditionPattern definitions.
    ///
    /// This allows developers to override default behavior for any tag
    /// or add completely new pattern processors without modifying the engine core.
    /// </summary>
    public static class OralTraditionProcessorRegistry
    {
        private static readonly Dictionary<string, IOralTraditionProcessor> _processors
            = new Dictionary<string, IOralTraditionProcessor>();

        static OralTraditionProcessorRegistry()
        {
            // Register built-in processors
            Register(new DefaultCallAndResponseProcessor());
            Register(new DefaultElderWisdomProcessor());
            Register(new DefaultSignifyingProcessor());
            Register(new DefaultTestimonyProcessor());
        }

        /// <summary>
        /// Registers a custom processor. Replaces any existing processor for the same role.
        /// </summary>
        public static void Register(IOralTraditionProcessor processor)
        {
            if (processor == null) return;
            _processors[processor.Role] = processor;
            Debug.Log($"[MNE] OralTraditionProcessor registered: {processor.Role}");
        }

        public static void Unregister(string role) => _processors.Remove(role);

        public static bool TryGet(string role, out IOralTraditionProcessor processor) =>
            _processors.TryGetValue(role, out processor);

        public static bool HasProcessor(string role) => _processors.ContainsKey(role);

        /// <summary>
        /// Runs all processors matching the node's cultural tags.
        /// Results are merged: prefixes/suffixes concatenated, requiresResponse OR-ed,
        /// response options combined.
        /// </summary>
        public static ProcessorResult RunAll(
            StoryNode node, StorySession session, CulturalContextBase context)
        {
            var merged = new ProcessorResult();
            if (node.culturalTags == null) return merged;

            foreach (var tag in node.culturalTags)
            {
                if (!_processors.TryGetValue(tag, out var processor)) continue;

                try
                {
                    var result = processor.Process(node, session, context);
                    if (result == null) continue;

                    merged.TextPrefix += result.TextPrefix;
                    merged.TextSuffix += result.TextSuffix;
                    if (result.RequiresResponse) merged.RequiresResponse = true;
                    merged.ResponseOptions.AddRange(result.ResponseOptions);
                    foreach (var kv in result.Metadata) merged.Metadata[kv.Key] = kv.Value;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[MNE] Processor '{tag}' threw: {ex.Message}");
                }
            }

            return merged;
        }
    }
}
