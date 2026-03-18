using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Root data model for a complete story deserialized from a .story.json file.
    /// </summary>
    [Serializable]
    public class StoryGraph
    {
        /// <summary>Human-readable title shown in the editor and UI.</summary>
        public string title;

        /// <summary>Short description shown in the story browser.</summary>
        public string description;

        /// <summary>Primary cultural context key (e.g., "black_american", "indigenous").</summary>
        public string culturalContext;

        /// <summary>Story version string for save-game compatibility checks.</summary>
        public string version = "1.0";

        /// <summary>Author credit displayed in-game.</summary>
        public string author;

        /// <summary>Node ID to begin playback from.</summary>
        public string startNodeId;

        /// <summary>All nodes in this story keyed by their unique ID.</summary>
        public List<StoryNode> nodes = new List<StoryNode>();

        // Runtime lookup built by StoryLoader after deserialization
        [NonSerialized]
        public Dictionary<string, StoryNode> nodeLookup;

        public void BuildLookup()
        {
            nodeLookup = new Dictionary<string, StoryNode>(nodes.Count);
            foreach (var node in nodes)
                nodeLookup[node.id] = node;
        }

        public StoryNode GetNode(string id)
        {
            if (nodeLookup == null) BuildLookup();
            nodeLookup.TryGetValue(id, out var node);
            return node;
        }
    }
}
