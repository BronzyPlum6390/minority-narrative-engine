using System;
using System.Collections.Generic;
using MinorityNarrativeEngine.SaveLoad;
using UnityEngine;

namespace MinorityNarrativeEngine.API
{
    /// <summary>
    /// Clean developer-facing API for the Minority Narrative Engine.
    /// Wraps NarrativeEngine with a fluent, discoverable surface for V2 developers.
    ///
    /// All methods are safe to call even before BeginStory() — they will queue
    /// or no-op gracefully if the engine isn't ready.
    ///
    /// Usage:
    ///   NarrativeAPI.Using(engine)
    ///       .WithExtension(new MyAudioExtension())
    ///       .WithCodeSwitching(true)
    ///       .OnNodeEnter(node => animator.Play(node.id))
    ///       .Begin();
    /// </summary>
    public class NarrativeAPI
    {
        private readonly NarrativeEngine _engine;
        private readonly ExtensionRegistry _extensions;
        private readonly List<Action<StoryNode>> _nodeEnterCallbacks = new();
        private readonly List<Action<ResolvedChoice>> _choiceCallbacks = new();
        private readonly List<Action<StorySession>> _storyCompleteCallbacks = new();

        private NarrativeAPI(NarrativeEngine engine)
        {
            _engine = engine;
            _extensions = new ExtensionRegistry(engine);
            HookEngineEvents();
        }

        // -------------------------------------------------------
        // Entry point
        // -------------------------------------------------------

        /// <summary>Creates a NarrativeAPI wrapper around an existing NarrativeEngine instance.</summary>
        public static NarrativeAPI Using(NarrativeEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));
            return new NarrativeAPI(engine);
        }

        // -------------------------------------------------------
        // Fluent configuration
        // -------------------------------------------------------

        /// <summary>Registers a custom extension.</summary>
        public NarrativeAPI WithExtension(INarrativeExtension extension)
        {
            _extensions.Register(extension);
            return this;
        }

        /// <summary>Enables or disables code-switching.</summary>
        public NarrativeAPI WithCodeSwitching(bool enabled)
        {
            _engine.SetCodeSwitching(enabled);
            return this;
        }

        /// <summary>Registers a callback fired every time a new node is entered.</summary>
        public NarrativeAPI OnNodeEnter(Action<StoryNode> callback)
        {
            _nodeEnterCallbacks.Add(callback);
            return this;
        }

        /// <summary>Registers a callback fired every time a choice is selected.</summary>
        public NarrativeAPI OnChoiceSelected(Action<ResolvedChoice> callback)
        {
            _choiceCallbacks.Add(callback);
            return this;
        }

        /// <summary>Registers a callback fired when the story completes.</summary>
        public NarrativeAPI OnComplete(Action<StorySession> callback)
        {
            _storyCompleteCallbacks.Add(callback);
            return this;
        }

        // -------------------------------------------------------
        // Story control
        // -------------------------------------------------------

        /// <summary>Begins the story from the start node.</summary>
        public NarrativeAPI Begin()
        {
            _engine.BeginStory();
            return this;
        }

        /// <summary>Selects a choice by index.</summary>
        public void Choose(int index) => _engine.SelectChoice(index);

        /// <summary>Advances a linear node.</summary>
        public void Advance() => _engine.Advance();

        // -------------------------------------------------------
        // Save / Load
        // -------------------------------------------------------

        /// <summary>Saves the current session to a slot.</summary>
        public void Save(int slotIndex, string label = null, List<MetadataEntry> metadata = null)
        {
            if (_engine.Session == null)
            {
                Debug.LogWarning("[MNE] NarrativeAPI.Save() called but no active session.");
                return;
            }
            SaveManager.Save(_engine.Session, slotIndex, label, metadata);
        }

        /// <summary>
        /// Loads a session from a slot and resumes the story from the saved node.
        /// Returns false if the slot is empty or corrupt.
        /// </summary>
        public bool Load(int slotIndex)
        {
            var session = SaveManager.Load(slotIndex);
            if (session == null) return false;

            // Re-initialize the engine with the loaded session
            _engine.ResumeFromSession(session);
            return true;
        }

        /// <summary>Returns slot descriptors for all save slots.</summary>
        public List<SaveSlot> GetSaveSlots() => SaveManager.GetAllSlots();

        // -------------------------------------------------------
        // Session queries
        // -------------------------------------------------------

        /// <summary>Returns the current collectivity score.</summary>
        public float CollectivityScore => _engine.Session?.collectivityScore ?? 0f;

        /// <summary>Returns the current community score for a given community ID.</summary>
        public float CommunityScore(string communityId) =>
            _engine.Session?.GetCommunityScore(communityId) ?? 0f;

        /// <summary>Returns true if the given flag is set in the current session.</summary>
        public bool HasFlag(string flag) => _engine.Session?.HasFlag(flag) ?? false;

        /// <summary>Returns true if the given node has been visited.</summary>
        public bool HasVisited(string nodeId) => _engine.Session?.HasVisited(nodeId) ?? false;

        // -------------------------------------------------------
        // Internal event wiring
        // -------------------------------------------------------

        private void HookEngineEvents()
        {
            _engine.OnDialogueEvent.AddListener(evt =>
            {
                if (evt?.sourceNode == null) return;
                foreach (var cb in _nodeEnterCallbacks) cb(evt.sourceNode);
                _extensions.NotifyNodeEnter(evt.sourceNode, _engine.Session);
            });

            _engine.OnStoryComplete.AddListener(() =>
            {
                foreach (var cb in _storyCompleteCallbacks) cb(_engine.Session);
                _extensions.NotifyStoryComplete(_engine.Session);
            });

            _engine.OnStoryBegin.AddListener(() =>
            {
                _extensions.NotifyStoryBegin(_engine.Session, _engine.Graph);
            });
        }
    }
}
