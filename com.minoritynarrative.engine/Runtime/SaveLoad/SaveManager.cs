using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MinorityNarrativeEngine.SaveLoad
{
    /// <summary>
    /// Manages saving and loading StorySession state to/from disk.
    ///
    /// Save files are written as JSON to Application.persistentDataPath.
    /// Supports multiple named slots, versioning, and automatic migration.
    ///
    /// Usage:
    ///   SaveManager.Save(session, slotIndex: 0, slotLabel: "Chapter 1");
    ///   var session = SaveManager.Load(slotIndex: 0);
    ///   var slots   = SaveManager.GetAllSlots();
    /// </summary>
    public static class SaveManager
    {
        public const int CURRENT_SAVE_VERSION = 2;
        public const int MAX_SLOTS = 10;

        private const string SaveFilePrefix = "mne_save_slot_";
        private const string SaveFileExtension = ".json";

        // -------------------------------------------------------
        // Save
        // -------------------------------------------------------

        /// <summary>
        /// Saves the current StorySession to the given slot index (0–9).
        /// </summary>
        public static void Save(
            StorySession session,
            int slotIndex,
            string slotLabel = null,
            List<MetadataEntry> metadata = null)
        {
            ValidateSlotIndex(slotIndex);

            var data = new SaveData
            {
                saveVersion  = CURRENT_SAVE_VERSION,
                engineVersion = "0.2.0",
                savedAtUtc   = DateTime.UtcNow.ToString("o"),
                slotLabel    = slotLabel ?? $"Save {slotIndex + 1}",
                storyTitle   = session.storyTitle,
                session      = session.TakeSnapshot(),
                metadata     = metadata ?? new List<MetadataEntry>()
            };

            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string path = GetSavePath(slotIndex);
            File.WriteAllText(path, json);
            Debug.Log($"[MNE] Saved to slot {slotIndex}: {path}");
        }

        // -------------------------------------------------------
        // Load
        // -------------------------------------------------------

        /// <summary>
        /// Loads a StorySession from the given slot index.
        /// Returns null if the slot is empty or the file is corrupt.
        /// </summary>
        public static StorySession Load(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);

            string path = GetSavePath(slotIndex);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[MNE] No save found at slot {slotIndex}.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);

                if (data?.session == null)
                    throw new Exception("SaveData or session snapshot is null after deserialization.");

                bool migrated = MigrationManager.Migrate(data);
                if (migrated)
                    File.WriteAllText(path, JsonUtility.ToJson(data, prettyPrint: true));

                var session = new StorySession();
                session.RestoreSnapshot(data.session);
                Debug.Log($"[MNE] Loaded slot {slotIndex}: '{data.storyTitle}'");
                return session;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MNE] Failed to load save slot {slotIndex}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads the full SaveData (including metadata) from a slot without constructing a session.
        /// Useful for displaying save info in a UI before committing to load.
        /// </summary>
        public static SaveData LoadRaw(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);
            string path = GetSavePath(slotIndex);
            if (!File.Exists(path)) return null;

            try
            {
                return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            }
            catch
            {
                return null;
            }
        }

        // -------------------------------------------------------
        // Slot management
        // -------------------------------------------------------

        /// <summary>
        /// Returns a lightweight SaveSlot descriptor for every slot (0–MAX_SLOTS).
        /// Slots with no file are returned with isEmpty = true.
        /// </summary>
        public static List<SaveSlot> GetAllSlots()
        {
            var slots = new List<SaveSlot>();
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                string path = GetSavePath(i);
                if (!File.Exists(path))
                {
                    slots.Add(new SaveSlot { slotIndex = i, isEmpty = true });
                    continue;
                }

                try
                {
                    var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                    slots.Add(new SaveSlot
                    {
                        slotIndex     = i,
                        slotLabel     = data.slotLabel,
                        storyTitle    = data.storyTitle,
                        savedAtUtc    = data.savedAtUtc,
                        engineVersion = data.engineVersion,
                        saveVersion   = data.saveVersion,
                        isEmpty       = false
                    });
                }
                catch
                {
                    slots.Add(new SaveSlot { slotIndex = i, isEmpty = true });
                }
            }
            return slots;
        }

        /// <summary>Deletes the save file for the given slot.</summary>
        public static void DeleteSlot(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[MNE] Deleted save slot {slotIndex}.");
            }
        }

        /// <summary>Returns true if a save file exists for the given slot.</summary>
        public static bool SlotExists(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);
            return File.Exists(GetSavePath(slotIndex));
        }

        // -------------------------------------------------------
        // Helpers
        // -------------------------------------------------------

        private static string GetSavePath(int slotIndex) =>
            Path.Combine(Application.persistentDataPath,
                $"{SaveFilePrefix}{slotIndex}{SaveFileExtension}");

        private static void ValidateSlotIndex(int index)
        {
            if (index < 0 || index >= MAX_SLOTS)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Slot index must be between 0 and {MAX_SLOTS - 1}.");
        }
    }
}
