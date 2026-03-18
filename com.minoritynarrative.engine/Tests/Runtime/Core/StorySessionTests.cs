using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class StorySessionTests
    {
        private StorySession _session;

        [SetUp]
        public void SetUp()
        {
            _session = new StorySession { storyTitle = "Test Story" };
        }

        // ── Flags ────────────────────────────────────────────────

        [Test]
        public void SetFlag_FlagIsSet()
        {
            _session.SetFlag("met_juniper");
            Assert.IsTrue(_session.HasFlag("met_juniper"));
        }

        [Test]
        public void ClearFlag_FlagIsRemoved()
        {
            _session.SetFlag("met_juniper");
            _session.ClearFlag("met_juniper");
            Assert.IsFalse(_session.HasFlag("met_juniper"));
        }

        [Test]
        public void HasFlag_UnsetFlag_ReturnsFalse()
        {
            Assert.IsFalse(_session.HasFlag("never_set_flag"));
        }

        [Test]
        public void SetFlag_MultipleFlagsCoexist()
        {
            _session.SetFlag("flag_a");
            _session.SetFlag("flag_b");
            Assert.IsTrue(_session.HasFlag("flag_a"));
            Assert.IsTrue(_session.HasFlag("flag_b"));
        }

        // ── Visited nodes ────────────────────────────────────────

        [Test]
        public void MarkVisited_NodeIsVisited()
        {
            _session.MarkVisited("node_001");
            Assert.IsTrue(_session.HasVisited("node_001"));
        }

        [Test]
        public void HasVisited_UnvisitedNode_ReturnsFalse()
        {
            Assert.IsFalse(_session.HasVisited("node_never_seen"));
        }

        // ── Relationships ────────────────────────────────────────

        [Test]
        public void AdjustRelationship_IncreasesScore()
        {
            _session.AdjustRelationship("isaiah", "juniper", 20f);
            Assert.AreEqual(20f, _session.GetRelationship("isaiah", "juniper"), 0.001f);
        }

        [Test]
        public void AdjustRelationship_IsSymmetric()
        {
            _session.AdjustRelationship("isaiah", "juniper", 30f);
            Assert.AreEqual(_session.GetRelationship("isaiah", "juniper"),
                            _session.GetRelationship("juniper", "isaiah"), 0.001f);
        }

        [Test]
        public void AdjustRelationship_ClampsAt100()
        {
            _session.AdjustRelationship("isaiah", "juniper", 200f);
            Assert.AreEqual(100f, _session.GetRelationship("isaiah", "juniper"), 0.001f);
        }

        [Test]
        public void AdjustRelationship_ClampsAtMinus100()
        {
            _session.AdjustRelationship("isaiah", "vance", -200f);
            Assert.AreEqual(-100f, _session.GetRelationship("isaiah", "vance"), 0.001f);
        }

        [Test]
        public void GetRelationship_UnknownPair_ReturnsZero()
        {
            Assert.AreEqual(0f, _session.GetRelationship("a", "b"), 0.001f);
        }

        // ── Community scores ─────────────────────────────────────

        [Test]
        public void AdjustCommunityScore_IncreasesScore()
        {
            _session.SetCommunityScore("freedens_town", 50f);
            _session.AdjustCommunityScore("freedens_town", 25f);
            Assert.AreEqual(75f, _session.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void AdjustCommunityScore_ClampsAt100()
        {
            _session.SetCommunityScore("freedens_town", 90f);
            _session.AdjustCommunityScore("freedens_town", 50f);
            Assert.AreEqual(100f, _session.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void AdjustCommunityScore_ClampsAtZero()
        {
            _session.SetCommunityScore("freedens_town", 10f);
            _session.AdjustCommunityScore("freedens_town", -50f);
            Assert.AreEqual(0f, _session.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void GetCommunityScore_Unknown_ReturnsZero()
        {
            Assert.AreEqual(0f, _session.GetCommunityScore("ghost_town"), 0.001f);
        }

        // ── Collectivity ─────────────────────────────────────────

        [Test]
        public void ApplyCollectivityDelta_Accumulates()
        {
            _session.ApplyCollectivityDelta(0.8f);
            _session.ApplyCollectivityDelta(0.5f);
            Assert.AreEqual(1.3f, _session.collectivityScore, 0.001f);
        }

        [Test]
        public void ApplyCollectivityDelta_Negative_Decreases()
        {
            _session.ApplyCollectivityDelta(-0.8f);
            Assert.AreEqual(-0.8f, _session.collectivityScore, 0.001f);
        }

        // ── Snapshot / Restore ───────────────────────────────────

        [Test]
        public void TakeSnapshot_CapturesFlags()
        {
            _session.SetFlag("chose_community");
            var snap = _session.TakeSnapshot();
            Assert.IsTrue(snap.flags.Contains("chose_community"));
        }

        [Test]
        public void TakeSnapshot_CapturesCollectivity()
        {
            _session.ApplyCollectivityDelta(1.5f);
            var snap = _session.TakeSnapshot();
            Assert.AreEqual(1.5f, snap.collectivityScore, 0.001f);
        }

        [Test]
        public void RestoreSnapshot_RestoresFlags()
        {
            _session.SetFlag("flag_to_restore");
            var snap = _session.TakeSnapshot();

            var newSession = new StorySession();
            newSession.RestoreSnapshot(snap);

            Assert.IsTrue(newSession.HasFlag("flag_to_restore"));
        }

        [Test]
        public void RestoreSnapshot_RestoresCommunityScore()
        {
            _session.SetCommunityScore("freedens_town", 75f);
            var snap = _session.TakeSnapshot();

            var newSession = new StorySession();
            newSession.RestoreSnapshot(snap);

            Assert.AreEqual(75f, newSession.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void RestoreSnapshot_RestoresCollectivity()
        {
            _session.ApplyCollectivityDelta(2.2f);
            var snap = _session.TakeSnapshot();

            var newSession = new StorySession();
            newSession.RestoreSnapshot(snap);

            Assert.AreEqual(2.2f, newSession.collectivityScore, 0.001f);
        }

        [Test]
        public void RoundTrip_SnapshotAndRestore_PreservesAllState()
        {
            _session.SetFlag("flag_a");
            _session.MarkVisited("node_001");
            _session.AdjustRelationship("isaiah", "juniper", 40f);
            _session.SetCommunityScore("freedens_town", 60f);
            _session.ApplyCollectivityDelta(1.0f);

            var snap = _session.TakeSnapshot();
            var restored = new StorySession();
            restored.RestoreSnapshot(snap);

            Assert.IsTrue(restored.HasFlag("flag_a"));
            Assert.IsTrue(restored.HasVisited("node_001"));
            Assert.AreEqual(40f, restored.GetRelationship("isaiah", "juniper"), 0.001f);
            Assert.AreEqual(60f, restored.GetCommunityScore("freedens_town"), 0.001f);
            Assert.AreEqual(1.0f, restored.collectivityScore, 0.001f);
        }
    }
}
