using NUnit.Framework;
using MinorityNarrativeEngine;
using MinorityNarrativeEngine.SaveLoad;

namespace MinorityNarrativeEngine.Tests
{
    public class MigrationManagerTests
    {
        [Test]
        public void Migrate_CurrentVersion_ReturnsFalse()
        {
            var data = new SaveData { saveVersion = SaveManager.CURRENT_SAVE_VERSION };
            bool migrated = MigrationManager.Migrate(data);
            Assert.IsFalse(migrated);
        }

        [Test]
        public void Migrate_CurrentVersion_DoesNotChangeVersion()
        {
            var data = new SaveData { saveVersion = SaveManager.CURRENT_SAVE_VERSION };
            MigrationManager.Migrate(data);
            Assert.AreEqual(SaveManager.CURRENT_SAVE_VERSION, data.saveVersion);
        }

        [Test]
        public void Migrate_OldVersion_ReturnsTrue()
        {
            var data = new SaveData
            {
                saveVersion = 1,
                session = new SessionSnapshot
                {
                    flags = new System.Collections.Generic.List<string>(),
                    visitedNodes = new System.Collections.Generic.List<string>(),
                    choiceHistory = new System.Collections.Generic.List<string>(),
                    variables = new System.Collections.Generic.List<StringFloatPair>(),
                    relationships = new System.Collections.Generic.List<StringFloatPair>(),
                    communityScores = new System.Collections.Generic.List<StringFloatPair>()
                }
            };
            bool migrated = MigrationManager.Migrate(data);
            Assert.IsTrue(migrated);
        }

        [Test]
        public void Migrate_V1_BumpsToCurrentVersion()
        {
            var data = new SaveData
            {
                saveVersion = 1,
                session = new SessionSnapshot
                {
                    flags = new System.Collections.Generic.List<string>(),
                    visitedNodes = new System.Collections.Generic.List<string>(),
                    choiceHistory = new System.Collections.Generic.List<string>(),
                    variables = new System.Collections.Generic.List<StringFloatPair>(),
                    relationships = new System.Collections.Generic.List<StringFloatPair>(),
                    communityScores = new System.Collections.Generic.List<StringFloatPair>()
                }
            };
            MigrationManager.Migrate(data);
            Assert.AreEqual(SaveManager.CURRENT_SAVE_VERSION, data.saveVersion);
        }
    }
}
