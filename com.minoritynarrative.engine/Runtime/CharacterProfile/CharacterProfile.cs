using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// ScriptableObject defining a named character's cultural identity, initial relationships,
    /// and portrait asset reference. Create via Assets > Create > Minority Narrative > Character Profile.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Character Profile")]
    public class CharacterProfile : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique ID used in story JSON files (e.g., 'isaiah_free').")]
        public string characterId;

        [Tooltip("Display name shown in UI dialogue boxes.")]
        public string displayName;

        [TextArea(2, 3)]
        [Tooltip("Brief character description for the story builder sidebar.")]
        public string description;

        [Header("Cultural Context")]
        [Tooltip("The primary cultural context for this character. Affects honorifics and code-switching.")]
        public CulturalContextBase culturalContext;

        [Header("Portrait")]
        [Tooltip("Portrait sprite asset ID or Addressables key for the UI layer to load.")]
        public string portraitId;

        [Header("Starting Relationships")]
        [Tooltip("Initial relationship scores with other characters at story start.")]
        public List<StartingRelationship> startingRelationships = new List<StartingRelationship>();

        [Tooltip("Default honorific tier this character holds in the community.")]
        public RelationshipTier communityTier = RelationshipTier.Neutral;
    }

    [System.Serializable]
    public class StartingRelationship
    {
        [Tooltip("Character ID of the other party.")]
        public string targetCharacterId;

        [Range(-100f, 100f)]
        [Tooltip("Starting relationship score. Positive = friendly/trusting.")]
        public float score;
    }
}
