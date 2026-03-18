using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;
using MinorityNarrativeEngine;
using MinorityNarrativeEngine.SaveLoad;

namespace MinorityNarrativeEngine.Tests
{
    public class SaveManagerTests
    {
        private StorySession _session;

        [SetUp]
        public void SetUp()
        {
            _session = new StorySession { storyTitle = "Test Story" };
            _session.SetFlag("test_flag");
            _session.SetCommunityScore("freedens_town", 75f);
            _session.ApplyCollectivityDelta(1.5f);
            _session.currentNodeId = "node_005";

            // Clean up any pre-existing test saves
            CleanSlot(0);
            CleanSlot(1);
        }

        [TearDown]
        public void TearDown()
        {
            CleanSlot(0);
            CleanSlot(1);
        }

        [Test]
        public void Save_CreatesFile()
        {
            SaveManager.Save(_session, 0, "Test Save");
            Assert.IsTrue(SaveManager.SlotExists(0));
        }

        [Test]
        public void Load_ReturnsSession_WithCorrectFlags()
        {
            SaveManager.Save(_session, 0);
            var loaded = SaveManager.Load(0);
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.HasFlag("test_flag"));
        }

        [Test]
        public void Load_ReturnsSession_WithCorrectCommunityScore()
        {
            SaveManager.Save(_session, 0);
            var loaded = SaveManager.Load(0);
            Assert.AreEqual(75f, loaded.GetCommunityScore("freedens_town"), 0.001f);
        }

        [Test]
        public void Load_ReturnsSession_WithCorrectCollectivity()
        {
            SaveManager.Save(_session, 0);
            var loaded = SaveManager.Load(0);
            Assert.AreEqual(1.5f, loaded.collectivityScore, 0.001f);
        }

        [Test]
        public void Load_ReturnsSession_WithCorrectCurrentNodeId()
        {
            SaveManager.Save(_session, 0);
            var loaded = SaveManager.Load(0);
            Assert.AreEqual("node_005", loaded.currentNodeId);
        }

        [Test]
        public void Load_EmptySlot_ReturnsNull()
        {
            var loaded = SaveManager.Load(1);
            Assert.IsNull(loaded);
        }

        [Test]
        public void DeleteSlot_RemovesFile()
        {
            SaveManager.Save(_session, 0);
            SaveManager.DeleteSlot(0);
            Assert.IsFalse(SaveManager.SlotExists(0));
        }

        [Test]
        public void GetAllSlots_ReturnsCorrectSlotCount()
        {
            var slots = SaveManager.GetAllSlots();
            Assert.AreEqual(SaveManager.MAX_SLOTS, slots.Count);
        }

        [Test]
        public void GetAllSlots_SavedSlot_IsNotEmpty()
        {
            SaveManager.Save(_session, 0, "My Save");
            var slots = SaveManager.GetAllSlots();
            Assert.IsFalse(slots[0].isEmpty);
            Assert.AreEqual("My Save", slots[0].slotLabel);
        }

        [Test]
        public void GetAllSlots_UnsavedSlot_IsEmpty()
        {
            var slots = SaveManager.GetAllSlots();
            Assert.IsTrue(slots[1].isEmpty);
        }

        [Test]
        public void Save_InvalidSlotIndex_ThrowsException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                SaveManager.Save(_session, -1));
        }

        [Test]
        public void Save_SlotIndexOverMax_ThrowsException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                SaveManager.Save(_session, SaveManager.MAX_SLOTS));
        }

        [Test]
        public void Load_CorruptFile_ReturnsNull()
        {
            // Write garbage to the slot path
            string path = Path.Combine(
                Application.persistentDataPath,
                $"mne_save_slot_0.json");
            File.WriteAllText(path, "{ this is not valid json {{{{");

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(@"\[MNE\] Failed to load save slot 0:.*"));
            var loaded = SaveManager.Load(0);
            Assert.IsNull(loaded);

            File.Delete(path);
        }

        private static void CleanSlot(int index)
        {
            if (SaveManager.SlotExists(index))
                SaveManager.DeleteSlot(index);
        }
    }
}
