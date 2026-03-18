using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Resolves character ID strings to their CharacterProfile ScriptableObjects.
    /// Assign your character assets here and reference this registry in NarrativeEngine.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Character Registry")]
    public class CharacterRegistry : ScriptableObject
    {
        [Tooltip("All character profiles available in your project. Add each character here.")]
        public List<CharacterProfile> characters = new List<CharacterProfile>();

        private Dictionary<string, CharacterProfile> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<string, CharacterProfile>();
            foreach (var c in characters)
            {
                if (c == null) continue;
                if (_lookup.ContainsKey(c.characterId))
                {
                    Debug.LogWarning($"[MNE] Duplicate character ID '{c.characterId}' in registry.");
                    continue;
                }
                _lookup[c.characterId] = c;
            }
        }

        public CharacterProfile Get(string characterId)
        {
            if (_lookup == null) Initialize();
            _lookup.TryGetValue(characterId, out var profile);
            return profile;
        }

        /// <summary>
        /// Seeds the StorySession with all starting relationships defined on character profiles.
        /// Call this before beginning a new playthrough.
        /// </summary>
        public void SeedSessionRelationships(StorySession session)
        {
            if (_lookup == null) Initialize();
            foreach (var character in characters)
            {
                if (character == null) continue;
                foreach (var rel in character.startingRelationships)
                {
                    session.SetRelationship(character.characterId, rel.targetCharacterId, rel.score);
                }
            }
        }
    }
}
