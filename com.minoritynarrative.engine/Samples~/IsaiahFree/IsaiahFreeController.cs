using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MinorityNarrativeEngine.Samples
{
    /// <summary>
    /// Demo game controller for "The Last Ride of Isaiah Free."
    /// Wires NarrativeEngine events to the UI prefab.
    ///
    /// Required UI references (assign in Inspector):
    ///   - speakerNameText     : TextMeshProUGUI
    ///   - dialogueBodyText    : TextMeshProUGUI
    ///   - choicePanel         : GameObject (parent of choice buttons)
    ///   - choiceButtonPrefab  : Button prefab with TextMeshProUGUI child
    ///   - continueButton      : Button (shown for linear/no-choice nodes)
    ///   - communityBar        : Slider (optional, shows Freedmen's Town score)
    ///   - collectivityBar     : Slider (optional, shows collectivity score)
    ///   - callResponsePanel   : GameObject (shown for call-and-response nodes)
    ///   - responseButtonParent: Transform (parent for response buttons)
    ///   - collectiveFrameLabel: TextMeshProUGUI (optional, shows "Community" or "Personal")
    /// </summary>
    [AddComponentMenu("Minority Narrative/Isaiah Free Demo Controller")]
    public class IsaiahFreeController : MonoBehaviour
    {
        [Header("Narrative Engine")]
        public NarrativeEngine narrativeEngine;

        [Header("Main Dialogue UI")]
        public TextMeshProUGUI speakerNameText;
        public TextMeshProUGUI dialogueBodyText;

        [Header("Choice UI")]
        public GameObject choicePanel;
        public Button choiceButtonPrefab;
        public Button continueButton;

        [Header("Community & Collectivity Indicators")]
        public Slider communityBar;
        public Slider collectivityBar;
        public TextMeshProUGUI collectiveFrameLabel;

        [Header("Call and Response UI")]
        public GameObject callResponsePanel;
        public Transform responseButtonParent;
        public Button responseButtonPrefab;

        [Header("Story Complete UI")]
        public GameObject storyCompletePanel;

        private List<Button> _choiceButtons = new List<Button>();
        private List<Button> _responseButtons = new List<Button>();

        private void Start()
        {
            if (narrativeEngine == null)
                narrativeEngine = FindObjectOfType<NarrativeEngine>();

            narrativeEngine.OnDialogueEvent.AddListener(OnDialogueEvent);
            narrativeEngine.OnStoryComplete.AddListener(OnStoryComplete);
            narrativeEngine.RegisterNamedEventListener("story_complete", OnStoryComplete);

            continueButton?.onClick.AddListener(OnContinue);

            SetupInitialState();
            narrativeEngine.BeginStory();
        }

        private void SetupInitialState()
        {
            choicePanel?.SetActive(false);
            continueButton?.gameObject.SetActive(false);
            callResponsePanel?.SetActive(false);
            storyCompletePanel?.SetActive(false);
        }

        // -------------------------------------------------------
        // Dialogue event handling
        // -------------------------------------------------------

        private void OnDialogueEvent(DialogueEvent evt)
        {
            if (evt == null) return;

            // Speaker name
            if (speakerNameText != null)
            {
                speakerNameText.text = string.IsNullOrEmpty(evt.speakerDisplayName)
                    ? "" : evt.speakerDisplayName.ToUpper();
            }

            // Dialogue text (animate characters for a typewriter feel)
            if (dialogueBodyText != null)
                StartCoroutine(TypewriterEffect(evt.resolvedText));

            // Choices or continue
            ClearChoices();
            if (evt.choices != null && evt.choices.Count > 0)
            {
                BuildChoiceButtons(evt);
            }
            else if (evt.sourceNode?.type != "end")
            {
                continueButton?.gameObject.SetActive(true);
            }

            // Call-and-response
            if (evt.isCallAndResponse)
                BuildResponseButtons(evt.suggestedResponses);
            else
                callResponsePanel?.SetActive(false);

            // Community + collectivity bars
            UpdateIndicators(evt);

            // Collectivity frame label
            if (collectiveFrameLabel != null)
            {
                collectiveFrameLabel.text = evt.collectiveFrame switch
                {
                    "collective"  => "COMMUNITY",
                    "individual"  => "PERSONAL",
                    "both"        => "COMMUNITY  ·  PERSONAL",
                    _             => ""
                };
            }
        }

        private void BuildChoiceButtons(DialogueEvent evt)
        {
            choicePanel?.SetActive(true);
            continueButton?.gameObject.SetActive(false);

            for (int i = 0; i < evt.choices.Count; i++)
            {
                var choice = evt.choices[i];
                var btn = Instantiate(choiceButtonPrefab, choicePanel.transform);
                _choiceButtons.Add(btn);

                // Main choice text
                var mainText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (mainText != null) mainText.text = choice.text;

                // Community + individual impact as sub-labels (if present)
                var impacts = btn.GetComponentsInChildren<TextMeshProUGUI>();
                if (impacts.Length > 1 && !string.IsNullOrEmpty(choice.communityImpact))
                    impacts[1].text = $"Community: {choice.communityImpact}";
                if (impacts.Length > 2 && !string.IsNullOrEmpty(choice.individualImpact))
                    impacts[2].text = $"Personal: {choice.individualImpact}";

                int capturedIndex = i;
                btn.onClick.AddListener(() => OnChoiceSelected(capturedIndex));
            }
        }

        private void BuildResponseButtons(List<string> responses)
        {
            if (responseButtonParent == null || responseButtonPrefab == null) return;
            foreach (var b in _responseButtons) if (b != null) Destroy(b.gameObject);
            _responseButtons.Clear();

            callResponsePanel?.SetActive(true);

            foreach (var response in responses ?? new List<string>())
            {
                var btn = Instantiate(responseButtonPrefab, responseButtonParent);
                var label = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = response;
                btn.onClick.AddListener(OnContinue);
                _responseButtons.Add(btn);
            }
        }

        private void ClearChoices()
        {
            foreach (var btn in _choiceButtons) if (btn != null) Destroy(btn.gameObject);
            _choiceButtons.Clear();
            choicePanel?.SetActive(false);
            continueButton?.gameObject.SetActive(false);
        }

        private void UpdateIndicators(DialogueEvent evt)
        {
            if (communityBar != null && narrativeEngine.Session != null)
            {
                float trust = narrativeEngine.Session.GetCommunityScore("freedens_town");
                communityBar.value = trust / 100f;
            }

            if (collectivityBar != null && narrativeEngine.Session != null)
            {
                // Map collectivity score from roughly -5..+5 to 0..1
                float normalised = Mathf.InverseLerp(-5f, 5f, narrativeEngine.Session.collectivityScore);
                collectivityBar.value = normalised;
            }
        }

        // -------------------------------------------------------
        // Player input handlers
        // -------------------------------------------------------

        private void OnChoiceSelected(int index)
        {
            narrativeEngine.SelectChoice(index);
        }

        private void OnContinue()
        {
            narrativeEngine.Advance();
        }

        private void OnStoryComplete()
        {
            ClearChoices();
            continueButton?.gameObject.SetActive(false);
            storyCompletePanel?.SetActive(true);
        }

        // -------------------------------------------------------
        // Typewriter effect
        // -------------------------------------------------------

        private Coroutine _typewriterCoroutine;
        private const float CharDelay = 0.02f;

        private IEnumerator TypewriterEffect(string text)
        {
            if (_typewriterCoroutine != null)
                StopCoroutine(_typewriterCoroutine);

            dialogueBodyText.text = "";
            foreach (char c in text)
            {
                dialogueBodyText.text += c;
                if (c != ' ') yield return new WaitForSeconds(CharDelay);
            }
        }
    }
}
