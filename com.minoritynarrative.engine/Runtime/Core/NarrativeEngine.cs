using System;
using UnityEngine;
using UnityEngine.Events;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Central MonoBehaviour orchestrator for the Minority Narrative Engine.
    /// Place on a GameObject in your scene and wire up your registries, story, and UI.
    ///
    /// Quickstart:
    ///   1. Assign your StoryJson TextAsset, CharacterRegistry, CommunityRegistry, CulturalContextRegistry.
    ///   2. Subscribe to OnDialogueEvent to receive resolved dialogue for your UI.
    ///   3. Call BeginStory() to start.
    ///   4. Call SelectChoice(index) or Advance() to progress.
    /// </summary>
    [AddComponentMenu("Minority Narrative/Narrative Engine")]
    public class NarrativeEngine : MonoBehaviour
    {
        // -------------------------------------------------------
        // Singleton (soft — does not survive scene loads by default)
        // -------------------------------------------------------

        public static NarrativeEngine Instance { get; private set; }

        // -------------------------------------------------------
        // Inspector configuration
        // -------------------------------------------------------

        [Header("Story")]
        [Tooltip("The .json TextAsset for your story file. Drag from the Project window.")]
        public TextAsset storyJson;

        [Header("Registries")]
        public CharacterRegistry characterRegistry;
        public CommunityRegistry communityRegistry;
        public CulturalContextRegistry culturalContextRegistry;

        [Header("Events")]
        [Tooltip("Raised each time a dialogue step is ready for the UI to display.")]
        public UnityEvent<DialogueEvent> OnDialogueEvent;

        [Tooltip("Raised when the story reaches an 'end' node or runs out of nodes.")]
        public UnityEvent OnStoryComplete;

        [Tooltip("Raised when the story first begins.")]
        public UnityEvent OnStoryBegin;

        // Named events fired by NodeTrigger("fire_event") — register via RegisterNamedEventListener()
        private System.Collections.Generic.Dictionary<string, UnityEvent> _namedEvents
            = new System.Collections.Generic.Dictionary<string, UnityEvent>();

        // -------------------------------------------------------
        // Runtime state
        // -------------------------------------------------------

        private StoryGraph _graph;
        private StorySession _session;
        private CulturalContextBase _activeContext;
        private DialogueSystem _dialogueSystem;
        private DialogueEvent _currentEvent;

        public StorySession Session => _session;
        public StoryGraph Graph => _graph;
        public CulturalContextBase ActiveContext => _activeContext;
        public DialogueEvent CurrentEvent => _currentEvent;

        // -------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // -------------------------------------------------------
        // Public API
        // -------------------------------------------------------

        /// <summary>
        /// Loads the assigned story JSON and begins playback from the start node.
        /// Call this from your game's "New Game" flow.
        /// </summary>
        public void BeginStory()
        {
            if (storyJson == null)
            {
                Debug.LogError("[MNE] NarrativeEngine: storyJson is not assigned.");
                return;
            }

            _graph = StoryLoader.LoadFromTextAsset(storyJson);
            _session = new StorySession { storyTitle = _graph.title };

            // Resolve cultural context from the story's declared context key
            _activeContext = null;
            if (culturalContextRegistry != null && !string.IsNullOrEmpty(_graph.culturalContext))
            {
                culturalContextRegistry.Initialize();
                _activeContext = culturalContextRegistry.Get(_graph.culturalContext);
            }

            // Seed session with starting relationships and community scores
            characterRegistry?.SeedSessionRelationships(_session);
            communityRegistry?.SeedSession(_session);

            // Initialize dialogue system
            _dialogueSystem = new DialogueSystem();
            _dialogueSystem.Initialize(_graph, _session, _activeContext, characterRegistry);

            OnStoryBegin?.Invoke();

            // Step to the first node
            EmitEvent(_dialogueSystem.StepToNode(_graph.startNodeId));
        }

        /// <summary>
        /// Advances through a linear node (no choices). Call when the player
        /// taps/clicks to continue past a non-choice dialogue line.
        /// </summary>
        public void Advance()
        {
            if (_dialogueSystem == null) return;
            var next = _dialogueSystem.Advance();
            if (next == null) CompleteStory();
            else EmitEvent(next);
        }

        /// <summary>
        /// Selects a choice by its index in the current resolved choice list.
        /// </summary>
        public void SelectChoice(int choiceIndex)
        {
            if (_currentEvent == null || _dialogueSystem == null) return;

            if (choiceIndex < 0 || choiceIndex >= _currentEvent.choices.Count)
            {
                Debug.LogWarning($"[MNE] Choice index {choiceIndex} out of range.");
                return;
            }

            var next = _dialogueSystem.SelectChoice(_currentEvent.choices[choiceIndex]);
            if (next == null) CompleteStory();
            else EmitEvent(next);
        }

        /// <summary>
        /// Resumes a story from a previously saved StorySession.
        /// The story JSON must already be assigned — only the session state is restored.
        /// </summary>
        public void ResumeFromSession(StorySession savedSession)
        {
            if (storyJson == null)
            {
                Debug.LogError("[MNE] ResumeFromSession: storyJson is not assigned.");
                return;
            }

            _graph = StoryLoader.LoadFromTextAsset(storyJson);

            _session = savedSession;

            _activeContext = null;
            if (culturalContextRegistry != null && !string.IsNullOrEmpty(_graph.culturalContext))
            {
                culturalContextRegistry.Initialize();
                _activeContext = culturalContextRegistry.Get(_graph.culturalContext);
            }

            _dialogueSystem = new DialogueSystem();
            _dialogueSystem.Initialize(_graph, _session, _activeContext, characterRegistry);

            OnStoryBegin?.Invoke();
            EmitEvent(_dialogueSystem.StepToNode(_session.currentNodeId ?? _graph.startNodeId));
        }

        /// <summary>Enable or disable code-switching at runtime (e.g., accessibility toggle).</summary>
        public void SetCodeSwitching(bool enabled)
        {
            if (_activeContext != null) _activeContext.codeSwitchingEnabled = enabled;
        }

        /// <summary>Register a listener for a named event fired by story triggers.</summary>
        public void RegisterNamedEventListener(string eventName, UnityAction listener)
        {
            if (!_namedEvents.TryGetValue(eventName, out var ue))
            {
                ue = new UnityEvent();
                _namedEvents[eventName] = ue;
            }
            ue.AddListener(listener);
        }

        /// <summary>Called by NodeTrigger to fire a named event.</summary>
        public void FireNamedEvent(string eventName)
        {
            if (_namedEvents.TryGetValue(eventName, out var ue))
                ue.Invoke();
        }

        // -------------------------------------------------------
        // Private
        // -------------------------------------------------------

        private void EmitEvent(DialogueEvent evt)
        {
            _currentEvent = evt;
            if (evt?.sourceNode?.type == "end")
            {
                OnDialogueEvent?.Invoke(evt);
                CompleteStory();
            }
            else
            {
                OnDialogueEvent?.Invoke(evt);
            }
        }

        private void CompleteStory()
        {
            _currentEvent = null;
            OnStoryComplete?.Invoke();
        }
    }
}
