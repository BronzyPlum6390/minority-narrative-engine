using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine.SaveLoad
{
    /// <summary>
    /// Top-level serializable container written to disk for a single save slot.
    /// Wraps SessionSnapshot with versioning and metadata for migration support.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>Save format version. Increment when the schema changes.</summary>
        public int saveVersion = SaveManager.CURRENT_SAVE_VERSION;

        /// <summary>Engine package version this save was created with.</summary>
        public string engineVersion = "0.2.0";

        /// <summary>UTC timestamp of when this save was written.</summary>
        public string savedAtUtc;

        /// <summary>Human-readable slot label shown in the save/load UI.</summary>
        public string slotLabel;

        /// <summary>Story title for display in save selection UI.</summary>
        public string storyTitle;

        /// <summary>The actual session state snapshot.</summary>
        public SessionSnapshot session;

        /// <summary>Arbitrary key-value pairs for game-specific metadata (e.g. chapter name, location).</summary>
        public List<MetadataEntry> metadata = new List<MetadataEntry>();
    }

    [Serializable]
    public class MetadataEntry
    {
        public string key;
        public string value;
    }
}
