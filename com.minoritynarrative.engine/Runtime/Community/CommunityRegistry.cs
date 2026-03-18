using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Holds all CommunityNode ScriptableObjects for a story.
    /// Seeds session scores on new game and provides lookup at runtime.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Community Registry")]
    public class CommunityRegistry : ScriptableObject
    {
        public List<CommunityNode> communities = new List<CommunityNode>();

        private Dictionary<string, CommunityNode> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<string, CommunityNode>();
            foreach (var c in communities)
            {
                if (c == null) continue;
                _lookup[c.communityId] = c;
            }
        }

        public CommunityNode Get(string communityId)
        {
            if (_lookup == null) Initialize();
            _lookup.TryGetValue(communityId, out var node);
            return node;
        }

        /// <summary>Seeds a new session with all starting community scores.</summary>
        public void SeedSession(StorySession session)
        {
            if (_lookup == null) Initialize();
            foreach (var community in communities)
            {
                if (community == null) continue;
                session.SetCommunityScore(community.communityId, community.startingScore);
            }
        }
    }
}
