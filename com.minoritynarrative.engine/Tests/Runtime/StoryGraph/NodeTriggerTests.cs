using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class NodeTriggerTests
    {
        private StorySession _session;

        [SetUp]
        public void SetUp()
        {
            _session = new StorySession();
            _session.SetCommunityScore("freedens_town", 50f);
        }

        [Test]
        public void SetFlag_SetsFlag()
        {
            var trigger = new NodeTrigger { type = "set_flag", key = "met_juniper" };
            trigger.Apply(_session);
            Assert.IsTrue(_session.HasFlag("met_juniper"));
        }

        [Test]
        public void ClearFlag_ClearsFlag()
        {
            _session.SetFlag("temp_flag");
            var trigger = new NodeTrigger { type = "clear_flag", key = "temp_flag" };
            trigger.Apply(_session);
            Assert.IsFalse(_session.HasFlag("temp_flag"));
        }

        [Test]
        public void AdjustCommunity_IncreasesScore()
        {
            var trigger = new NodeTrigger { type = "adjust_community", key = "freedens_town", value = 20f };
            trigger.Apply(_session);
            Assert.AreEqual(70f, _session.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void AdjustCommunity_DecreasesScore()
        {
            var trigger = new NodeTrigger { type = "adjust_community", key = "freedens_town", value = -30f };
            trigger.Apply(_session);
            Assert.AreEqual(20f, _session.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void AdjustRelationship_UpdatesScore()
        {
            var trigger = new NodeTrigger
            {
                type = "adjust_relationship",
                key = "isaiah",
                relatedCharacterId = "juniper",
                value = 25f
            };
            trigger.Apply(_session);
            Assert.AreEqual(25f, _session.GetRelationship("isaiah", "juniper"), 0.001f);
        }

        [Test]
        public void MultipleTriggers_ApplyInSequence()
        {
            new NodeTrigger { type = "set_flag", key = "flag_one" }.Apply(_session);
            new NodeTrigger { type = "adjust_community", key = "freedens_town", value = 10f }.Apply(_session);
            new NodeTrigger { type = "adjust_relationship", key = "isaiah", relatedCharacterId = "ma_ellen", value = 30f }.Apply(_session);

            Assert.IsTrue(_session.HasFlag("flag_one"));
            Assert.AreEqual(60f, _session.GetCommunityScore("freedens_town"), 0.001f);
            Assert.AreEqual(30f, _session.GetRelationship("isaiah", "ma_ellen"), 0.001f);
        }
    }
}
