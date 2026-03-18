using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// ScriptableObject representing a community, settlement, or collective body in the story world.
    /// Community health/trust scores are tracked per-session in StorySession, but the
    /// ScriptableObject defines the community's identity and starting score.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Community Node")]
    public class CommunityNode : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique ID used in story JSON triggers and conditions (e.g., 'freedens_town').")]
        public string communityId;

        [Tooltip("Display name shown in UI community indicators.")]
        public string displayName;

        [TextArea(2, 3)]
        [Tooltip("Brief description for the story builder sidebar.")]
        public string description;

        [Header("Starting State")]
        [Range(0f, 100f)]
        [Tooltip("Starting trust/health score at the beginning of a new playthrough.")]
        public float startingScore = 50f;

        [Header("Thresholds (for conditional logic)")]
        [Range(0f, 100f)]
        [Tooltip("Score below this means the community is in crisis. Unlocks crisis-specific story branches.")]
        public float crisisThreshold = 20f;

        [Range(0f, 100f)]
        [Tooltip("Score above this means the community is thriving. Unlocks positive story branches.")]
        public float thrivingThreshold = 75f;

        [Header("Narrative")]
        [Tooltip("Displayed when community health is high.")]
        public string thrivingStateText;

        [Tooltip("Displayed when community health is in crisis.")]
        public string crisisStateText;
    }
}
