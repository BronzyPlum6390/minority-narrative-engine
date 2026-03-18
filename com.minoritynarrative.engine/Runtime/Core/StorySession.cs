using System;
using System.Collections.Generic;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Holds the full mutable state of an active playthrough.
    /// Passed to conditions and triggers so they can read and mutate session state.
    /// </summary>
    public class StorySession
    {
        public string storyTitle;
        public string currentNodeId;

        // --- Narrative state ---

        private readonly HashSet<string> _flags = new HashSet<string>();
        private readonly HashSet<string> _visitedNodes = new HashSet<string>();
        private readonly Dictionary<string, float> _variables = new Dictionary<string, float>();

        // --- Relationship state: "charA::charB" -> score (-100 to 100) ---
        private readonly Dictionary<string, float> _relationships = new Dictionary<string, float>();

        // --- Community trust/health scores: communityId -> score (0 to 100) ---
        private readonly Dictionary<string, float> _communityScores = new Dictionary<string, float>();

        // --- Collectivity tracking ---

        /// <summary>
        /// Running sum of collectivity deltas from all choices made this session.
        /// Positive = player has trended collective/community. Negative = individual.
        /// </summary>
        public float collectivityScore { get; private set; }

        // --- Choice history ---
        public List<string> choiceHistory { get; } = new List<string>();

        // -------------------------------------------------------
        // Flags
        // -------------------------------------------------------

        public void SetFlag(string flag) => _flags.Add(flag);
        public void ClearFlag(string flag) => _flags.Remove(flag);
        public bool HasFlag(string flag) => _flags.Contains(flag);

        // -------------------------------------------------------
        // Visited nodes
        // -------------------------------------------------------

        public void MarkVisited(string nodeId) => _visitedNodes.Add(nodeId);
        public bool HasVisited(string nodeId) => _visitedNodes.Contains(nodeId);

        // -------------------------------------------------------
        // Variables
        // -------------------------------------------------------

        public void SetVariable(string key, float value) => _variables[key] = value;

        public float GetVariable(string key) =>
            _variables.TryGetValue(key, out float v) ? v : 0f;

        // -------------------------------------------------------
        // Relationships
        // -------------------------------------------------------

        private static string RelKey(string a, string b) =>
            string.Compare(a, b, StringComparison.Ordinal) <= 0 ? $"{a}::{b}" : $"{b}::{a}";

        public float GetRelationship(string charA, string charB)
        {
            _relationships.TryGetValue(RelKey(charA, charB), out float score);
            return score;
        }

        public void AdjustRelationship(string charA, string charB, float delta)
        {
            var key = RelKey(charA, charB);
            _relationships.TryGetValue(key, out float current);
            _relationships[key] = UnityEngine.Mathf.Clamp(current + delta, -100f, 100f);
        }

        public void SetRelationship(string charA, string charB, float score)
        {
            _relationships[RelKey(charA, charB)] = UnityEngine.Mathf.Clamp(score, -100f, 100f);
        }

        // -------------------------------------------------------
        // Community scores
        // -------------------------------------------------------

        public float GetCommunityScore(string communityId)
        {
            _communityScores.TryGetValue(communityId, out float score);
            return score;
        }

        public void AdjustCommunityScore(string communityId, float delta)
        {
            _communityScores.TryGetValue(communityId, out float current);
            _communityScores[communityId] = UnityEngine.Mathf.Clamp(current + delta, 0f, 100f);
        }

        public void SetCommunityScore(string communityId, float score)
        {
            _communityScores[communityId] = UnityEngine.Mathf.Clamp(score, 0f, 100f);
        }

        // -------------------------------------------------------
        // Collectivity
        // -------------------------------------------------------

        public void ApplyCollectivityDelta(float delta)
        {
            collectivityScore += delta;
        }

        // -------------------------------------------------------
        // Snapshot (for save/load)
        // -------------------------------------------------------

        public SessionSnapshot TakeSnapshot() => new SessionSnapshot
        {
            storyTitle = storyTitle,
            currentNodeId = currentNodeId,
            collectivityScore = collectivityScore,
            flags = new List<string>(_flags),
            visitedNodes = new List<string>(_visitedNodes),
            choiceHistory = new List<string>(choiceHistory),
            variables = ToSerializableList(_variables),
            relationships = ToSerializableList(_relationships),
            communityScores = ToSerializableList(_communityScores)
        };

        public void RestoreSnapshot(SessionSnapshot snap)
        {
            storyTitle = snap.storyTitle;
            currentNodeId = snap.currentNodeId;
            collectivityScore = snap.collectivityScore;
            _flags.Clear(); if (snap.flags != null) foreach (var f in snap.flags) _flags.Add(f);
            _visitedNodes.Clear(); if (snap.visitedNodes != null) foreach (var v in snap.visitedNodes) _visitedNodes.Add(v);
            choiceHistory.Clear(); if (snap.choiceHistory != null) choiceHistory.AddRange(snap.choiceHistory);
            _variables.Clear(); if (snap.variables != null) foreach (var p in snap.variables) _variables[p.key] = p.value;
            _relationships.Clear(); if (snap.relationships != null) foreach (var p in snap.relationships) _relationships[p.key] = p.value;
            _communityScores.Clear(); if (snap.communityScores != null) foreach (var p in snap.communityScores) _communityScores[p.key] = p.value;
        }

        private static List<StringFloatPair> ToSerializableList(Dictionary<string, float> dict)
        {
            var list = new List<StringFloatPair>(dict.Count);
            foreach (var kv in dict)
                list.Add(new StringFloatPair { key = kv.Key, value = kv.Value });
            return list;
        }
    }

    /// <summary>Serializable key-value pair for float maps (JsonUtility does not support Dictionary).</summary>
    [Serializable]
    public class StringFloatPair
    {
        public string key;
        public float value;
    }

    /// <summary>Serializable snapshot of a StorySession for save/load.</summary>
    [Serializable]
    public class SessionSnapshot
    {
        public string storyTitle;
        public string currentNodeId;
        public float collectivityScore;
        public List<string> flags;
        public List<string> visitedNodes;
        public List<string> choiceHistory;
        public List<StringFloatPair> variables;
        public List<StringFloatPair> relationships;
        public List<StringFloatPair> communityScores;
    }
}
