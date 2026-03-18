using UnityEngine;

namespace MinorityNarrativeEngine.SaveLoad
{
    /// <summary>
    /// Handles migration of SaveData from older save format versions to the current one.
    /// Add a new case to Migrate() each time CURRENT_SAVE_VERSION is incremented.
    /// </summary>
    public static class MigrationManager
    {
        /// <summary>
        /// Migrates save data to the current version in place.
        /// Returns true if migration was performed, false if data was already current.
        /// </summary>
        public static bool Migrate(SaveData data)
        {
            if (data.saveVersion == SaveManager.CURRENT_SAVE_VERSION)
                return false;

            Debug.Log($"[MNE] Migrating save data from version {data.saveVersion} to {SaveManager.CURRENT_SAVE_VERSION}");

            while (data.saveVersion < SaveManager.CURRENT_SAVE_VERSION)
            {
                switch (data.saveVersion)
                {
                    case 1:
                        MigrateV1ToV2(data);
                        break;

                    // Add future migrations here:
                    // case 2:
                    //     MigrateV2ToV3(data);
                    //     break;

                    default:
                        Debug.LogWarning($"[MNE] No migration path from save version {data.saveVersion}. Skipping.");
                        data.saveVersion = SaveManager.CURRENT_SAVE_VERSION;
                        break;
                }
            }

            return true;
        }

        // -------------------------------------------------------
        // Migration steps
        // -------------------------------------------------------

        /// <summary>
        /// V1 → V2: collectivityScore field added to SessionSnapshot.
        /// Existing saves default to 0 (neutral).
        /// </summary>
        private static void MigrateV1ToV2(SaveData data)
        {
            // collectivityScore is initialized to 0 by default on deserialization —
            // no explicit field manipulation needed. Just bump the version.
            data.saveVersion = 2;
            Debug.Log("[MNE] Migration V1 → V2 complete.");
        }
    }
}
