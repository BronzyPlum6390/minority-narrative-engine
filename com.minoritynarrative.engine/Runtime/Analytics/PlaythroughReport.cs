using System;
using System.Collections.Generic;
using System.Linq;

namespace MinorityNarrativeEngine.Analytics
{
    /// <summary>
    /// Summary report generated from a completed or in-progress playthrough.
    /// Surfaced by AnalyticsRecorder.GenerateReport().
    /// </summary>
    [Serializable]
    public class PlaythroughReport
    {
        public string storyTitle;
        public string startedAtUtc;
        public string completedAtUtc;
        public bool storyCompleted;

        public int totalNodesVisited;
        public int totalChoicesMade;
        public float finalCollectivityScore;
        public float collectivityTrend; // average delta across all choices

        public List<string> visitedNodeIds = new List<string>();
        public List<string> choicesMade = new List<string>(); // "nodeId → targetNodeId"
        public List<string> flagsSet = new List<string>();

        public List<AnalyticsEvent> events = new List<AnalyticsEvent>();

        // -------------------------------------------------------
        // Derived insights
        // -------------------------------------------------------

        /// <summary>The node the player spent the most time on (highest dwell time).</summary>
        public string MostDwelledNodeId
        {
            get
            {
                AnalyticsEvent best = null;
                foreach (var e in events)
                    if (best == null || e.dwellTimeMs > best.dwellTimeMs) best = e;
                return best?.nodeId;
            }
        }

        /// <summary>Distribution of cultural tags across all visited nodes.</summary>
        public Dictionary<string, int> CulturalTagFrequency
        {
            get
            {
                var freq = new Dictionary<string, int>();
                foreach (var e in events)
                {
                    if (e.culturalTags == null) continue;
                    foreach (var tag in e.culturalTags)
                    {
                        freq.TryGetValue(tag, out int count);
                        freq[tag] = count + 1;
                    }
                }
                return freq;
            }
        }

        /// <summary>
        /// Collectivity stance label based on final score.
        /// </summary>
        public string CollectivityStanceLabel
        {
            get
            {
                return finalCollectivityScore switch
                {
                    >= 3f  => "Strongly Collective",
                    >= 1f  => "Community-Leaning",
                    >= -1f => "Balanced",
                    >= -3f => "Individual-Leaning",
                    _      => "Strongly Individual"
                };
            }
        }

        public string ToJson() => UnityEngine.JsonUtility.ToJson(this, prettyPrint: true);
    }
}
