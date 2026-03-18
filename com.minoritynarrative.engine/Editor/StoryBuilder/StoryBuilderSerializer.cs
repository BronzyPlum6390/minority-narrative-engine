using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Converts a StoryGraph to/from the canonical .story.json format.
    /// Uses Unity's JsonUtility for round-trip fidelity.
    /// </summary>
    public class StoryBuilderSerializer
    {
        public string Serialize(StoryGraph graph)
        {
            return JsonUtility.ToJson(graph, prettyPrint: true);
        }

        public StoryGraph Deserialize(string json)
        {
            return StoryLoader.LoadFromJson(json);
        }
    }
}
