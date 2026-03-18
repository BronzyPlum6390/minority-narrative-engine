using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class NodeConditionTests
    {
        private StorySession _session;

        [SetUp]
        public void SetUp()
        {
            _session = new StorySession();
            _session.SetFlag("active_flag");
            _session.SetCommunityScore("freedens_town", 60f);
            _session.AdjustRelationship("isaiah", "juniper", 40f);
            _session.ApplyCollectivityDelta(1.5f);
            _session.MarkVisited("node_intro");
        }

        // ── Flag conditions ──────────────────────────────────────

        [Test]
        public void FlagCondition_Set_PassesWhenFlagIsSet()
        {
            var cond = new NodeCondition { type = "flag", key = "active_flag", op = "set" };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void FlagCondition_Unset_PassesWhenFlagIsNotSet()
        {
            var cond = new NodeCondition { type = "flag", key = "not_set_flag", op = "unset" };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void FlagCondition_Set_FailsWhenFlagNotSet()
        {
            var cond = new NodeCondition { type = "flag", key = "not_set_flag", op = "set" };
            Assert.IsFalse(cond.Evaluate(_session));
        }

        // ── Community conditions ─────────────────────────────────

        [Test]
        public void CommunityCondition_Gte_PassesWhenScoreAboveThreshold()
        {
            var cond = new NodeCondition { type = "community", key = "freedens_town", op = "gte", value = 50f };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void CommunityCondition_Gte_FailsWhenScoreBelowThreshold()
        {
            var cond = new NodeCondition { type = "community", key = "freedens_town", op = "gte", value = 80f };
            Assert.IsFalse(cond.Evaluate(_session));
        }

        [Test]
        public void CommunityCondition_Lt_PassesWhenScoreBelow()
        {
            var cond = new NodeCondition { type = "community", key = "freedens_town", op = "lt", value = 70f };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        // ── Relationship conditions ──────────────────────────────

        [Test]
        public void RelationshipCondition_Gte_PassesWhenScoreMet()
        {
            var cond = new NodeCondition
            {
                type = "relationship",
                key = "isaiah",
                relatedCharacterId = "juniper",
                op = "gte",
                value = 30f
            };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void RelationshipCondition_Gte_FailsWhenScoreNotMet()
        {
            var cond = new NodeCondition
            {
                type = "relationship",
                key = "isaiah",
                relatedCharacterId = "juniper",
                op = "gte",
                value = 60f
            };
            Assert.IsFalse(cond.Evaluate(_session));
        }

        // ── Collectivity conditions ──────────────────────────────

        [Test]
        public void CollectivityCondition_Gte_PassesWhenScoreMet()
        {
            var cond = new NodeCondition { type = "collectivity", key = "collectivity", op = "gte", value = 1.0f };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void CollectivityCondition_Lt_FailsWhenScoreAbove()
        {
            var cond = new NodeCondition { type = "collectivity", key = "collectivity", op = "lt", value = 1.0f };
            Assert.IsFalse(cond.Evaluate(_session));
        }

        // ── Visited conditions ───────────────────────────────────

        [Test]
        public void VisitedCondition_Set_PassesForVisitedNode()
        {
            var cond = new NodeCondition { type = "visited", key = "node_intro", op = "set" };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void VisitedCondition_Unset_PassesForUnvisitedNode()
        {
            var cond = new NodeCondition { type = "visited", key = "node_secret", op = "unset" };
            Assert.IsTrue(cond.Evaluate(_session));
        }

        [Test]
        public void VisitedCondition_Set_FailsForUnvisitedNode()
        {
            var cond = new NodeCondition { type = "visited", key = "node_secret", op = "set" };
            Assert.IsFalse(cond.Evaluate(_session));
        }

        // ── Unknown type ─────────────────────────────────────────

        [Test]
        public void UnknownConditionType_ReturnsTrue()
        {
            var cond = new NodeCondition { type = "unknown_type", key = "anything", op = "set" };
            Assert.IsTrue(cond.Evaluate(_session));
        }
    }
}
