using NUnit.Framework;
using System.Collections.Generic;
using MinorityNarrativeEngine.Analytics;

namespace MinorityNarrativeEngine.Tests
{
    public class AnalyticsReportTests
    {
        [Test]
        public void CollectivityStanceLabel_StronglyCollective()
        {
            var report = new PlaythroughReport { finalCollectivityScore = 4f };
            Assert.AreEqual("Strongly Collective", report.CollectivityStanceLabel);
        }

        [Test]
        public void CollectivityStanceLabel_CommunityLeaning()
        {
            var report = new PlaythroughReport { finalCollectivityScore = 2f };
            Assert.AreEqual("Community-Leaning", report.CollectivityStanceLabel);
        }

        [Test]
        public void CollectivityStanceLabel_Balanced()
        {
            var report = new PlaythroughReport { finalCollectivityScore = 0f };
            Assert.AreEqual("Balanced", report.CollectivityStanceLabel);
        }

        [Test]
        public void CollectivityStanceLabel_IndividualLeaning()
        {
            var report = new PlaythroughReport { finalCollectivityScore = -2f };
            Assert.AreEqual("Individual-Leaning", report.CollectivityStanceLabel);
        }

        [Test]
        public void CollectivityStanceLabel_StronglyIndividual()
        {
            var report = new PlaythroughReport { finalCollectivityScore = -4f };
            Assert.AreEqual("Strongly Individual", report.CollectivityStanceLabel);
        }

        [Test]
        public void CulturalTagFrequency_CountsCorrectly()
        {
            var report = new PlaythroughReport
            {
                events = new List<AnalyticsEvent>
                {
                    new AnalyticsEvent { culturalTags = new[] { "elder_wisdom", "testimony" } },
                    new AnalyticsEvent { culturalTags = new[] { "elder_wisdom" } },
                    new AnalyticsEvent { culturalTags = new[] { "signifying" } }
                }
            };

            var freq = report.CulturalTagFrequency;
            Assert.AreEqual(2, freq["elder_wisdom"]);
            Assert.AreEqual(1, freq["testimony"]);
            Assert.AreEqual(1, freq["signifying"]);
        }

        [Test]
        public void CulturalTagFrequency_NullTags_DoesNotThrow()
        {
            var report = new PlaythroughReport
            {
                events = new List<AnalyticsEvent>
                {
                    new AnalyticsEvent { culturalTags = null },
                    new AnalyticsEvent { culturalTags = new[] { "testimony" } }
                }
            };

            Assert.DoesNotThrow(() => _ = report.CulturalTagFrequency);
        }

        [Test]
        public void MostDwelledNodeId_ReturnsHighestDwellNode()
        {
            var report = new PlaythroughReport
            {
                events = new List<AnalyticsEvent>
                {
                    new AnalyticsEvent { type = "node_enter", nodeId = "node_a", dwellTimeMs = 1000 },
                    new AnalyticsEvent { type = "node_enter", nodeId = "node_b", dwellTimeMs = 5000 },
                    new AnalyticsEvent { type = "node_enter", nodeId = "node_c", dwellTimeMs = 2000 }
                }
            };

            Assert.AreEqual("node_b", report.MostDwelledNodeId);
        }

        [Test]
        public void ToJson_ReturnsNonEmptyString()
        {
            var report = new PlaythroughReport
            {
                storyTitle = "Test",
                totalChoicesMade = 3,
                finalCollectivityScore = 1.5f
            };

            string json = report.ToJson();
            Assert.IsNotEmpty(json);
            StringAssert.Contains("Test", json);
        }
    }
}
