using System;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Deserializes a story JSON file into a StoryGraph and validates its structure.
    /// </summary>
    public static class StoryLoader
    {
        /// <summary>
        /// Loads a story from a TextAsset (drag a .json file into Resources or use Addressables).
        /// </summary>
        public static StoryGraph LoadFromTextAsset(TextAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            return ParseJson(asset.text, asset.name);
        }

        /// <summary>
        /// Loads a story from a raw JSON string.
        /// </summary>
        public static StoryGraph LoadFromJson(string json, string debugName = "story")
        {
            return ParseJson(json, debugName);
        }

        /// <summary>
        /// Loads a story from Resources by path (no extension needed).
        /// </summary>
        public static StoryGraph LoadFromResources(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
                throw new Exception($"[MNE] Story not found in Resources at: {resourcePath}");
            return LoadFromTextAsset(asset);
        }

        private static StoryGraph ParseJson(string json, string debugName)
        {
            try
            {
                var graph = JsonUtility.FromJson<StoryGraph>(json);
                Validate(graph, debugName);
                graph.BuildLookup();
                return graph;
            }
            catch (Exception ex) when (!(ex is StoryValidationException))
            {
                throw new Exception($"[MNE] Failed to parse story '{debugName}': {ex.Message}", ex);
            }
        }

        private static void Validate(StoryGraph graph, string name)
        {
            if (string.IsNullOrEmpty(graph.startNodeId))
                throw new StoryValidationException(name, "startNodeId is required");

            if (graph.nodes == null || graph.nodes.Count == 0)
                throw new StoryValidationException(name, "story has no nodes");

            graph.BuildLookup();

            if (!graph.nodeLookup.ContainsKey(graph.startNodeId))
                throw new StoryValidationException(name, $"startNodeId '{graph.startNodeId}' not found in nodes");

            foreach (var node in graph.nodes)
            {
                if (string.IsNullOrEmpty(node.id))
                    throw new StoryValidationException(name, "a node is missing its id field");

                foreach (var choice in node.choices)
                {
                    if (!string.IsNullOrEmpty(choice.targetNodeId) &&
                        !graph.nodeLookup.ContainsKey(choice.targetNodeId))
                    {
                        Debug.LogWarning($"[MNE] Story '{name}': choice in node '{node.id}' targets unknown node '{choice.targetNodeId}'");
                    }
                }

                if (!string.IsNullOrEmpty(node.nextNodeId) &&
                    !graph.nodeLookup.ContainsKey(node.nextNodeId))
                {
                    Debug.LogWarning($"[MNE] Story '{name}': node '{node.id}' has unknown nextNodeId '{node.nextNodeId}'");
                }
            }
        }
    }

    public class StoryValidationException : Exception
    {
        public StoryValidationException(string storyName, string issue)
            : base($"[MNE] Story validation failed for '{storyName}': {issue}") { }
    }
}
