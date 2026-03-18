using System;

namespace MinorityNarrativeEngine.SaveLoad
{
    /// <summary>
    /// Lightweight descriptor for a save slot shown in the save/load UI.
    /// Loaded from disk without deserializing the full SessionSnapshot.
    /// </summary>
    [Serializable]
    public class SaveSlot
    {
        public int slotIndex;
        public string slotLabel;
        public string storyTitle;
        public string savedAtUtc;
        public string engineVersion;
        public int saveVersion;
        public bool isEmpty;

        public string FormattedDate
        {
            get
            {
                if (string.IsNullOrEmpty(savedAtUtc)) return "—";
                if (DateTime.TryParse(savedAtUtc, out var dt))
                    return dt.ToLocalTime().ToString("MMM d, yyyy  h:mm tt");
                return savedAtUtc;
            }
        }
    }
}
