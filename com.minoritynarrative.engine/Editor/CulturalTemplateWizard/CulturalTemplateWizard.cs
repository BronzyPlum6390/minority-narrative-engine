using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Multi-step wizard for non-technical creators to configure a cultural context.
    /// Open via: Window > Minority Narrative > Cultural Template Wizard
    ///
    /// Steps:
    ///   1. Select a cultural context base
    ///   2. Configure honorifics
    ///   3. Configure code-switching
    ///   4. Set collectivity weight
    ///   5. Review and generate the ScriptableObject asset
    /// </summary>
    public class CulturalTemplateWizard : EditorWindow
    {
        private readonly List<WizardStep> _steps = new List<WizardStep>();
        private int _currentStep;
        private CulturalContextBase _workingContext;

        [MenuItem("Window/Minority Narrative/Cultural Template Wizard")]
        public static void Open()
        {
            var window = GetWindow<CulturalTemplateWizard>("Cultural Template Wizard");
            window.minSize = new Vector2(580, 500);
            window.Initialize();
            window.Show();
        }

        private void Initialize()
        {
            _steps.Clear();
            _steps.Add(new SelectContextStep());
            _steps.Add(new HonorificsConfigStep());
            _steps.Add(new CodeSwitchingConfigStep());
            _steps.Add(new CollectivityConfigStep());
            _steps.Add(new ReviewAndGenerateStep());

            foreach (var step in _steps)
                step.wizard = this;

            _currentStep = 0;
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawStepIndicator();
            EditorGUILayout.Space(8);

            if (_steps.Count == 0) Initialize();
            if (_currentStep >= _steps.Count) return;

            var step = _steps[_currentStep];
            step.OnGUI(_workingContext);

            EditorGUILayout.Space(8);
            DrawNavigation(step);
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Cultural Template Wizard", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            EditorGUILayout.HelpBox(
                "This wizard helps you configure a cultural context for your story. " +
                "You don't need to write any code — just answer the questions on each page.",
                MessageType.Info);
            EditorGUILayout.Space(4);
        }

        private void DrawStepIndicator()
        {
            string[] stepNames = { "Choose Context", "Honorifics", "Language", "Community Weight", "Generate" };
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < stepNames.Length; i++)
            {
                bool isCurrent = i == _currentStep;
                bool isDone = i < _currentStep;
                GUI.color = isCurrent ? new Color(0.4f, 0.8f, 0.6f) : isDone ? Color.gray : Color.white;
                GUILayout.Label($"{i + 1}. {stepNames[i]}", isCurrent ? EditorStyles.boldLabel : EditorStyles.miniLabel);
                GUI.color = Color.white;
                if (i < stepNames.Length - 1) GUILayout.Label("→", EditorStyles.miniLabel, GUILayout.Width(16));
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNavigation(WizardStep step)
        {
            EditorGUILayout.BeginHorizontal();

            if (_currentStep > 0 && GUILayout.Button("← Back", GUILayout.Width(90)))
            {
                _currentStep--;
                Repaint();
            }

            GUILayout.FlexibleSpace();

            bool isLast = _currentStep == _steps.Count - 1;
            string nextLabel = isLast ? "Generate Asset" : "Next →";

            bool canAdvance = step.Validate(_workingContext);
            GUI.enabled = canAdvance;
            if (GUILayout.Button(nextLabel, GUILayout.Width(130)))
            {
                _workingContext = step.Apply(_workingContext);
                if (!isLast) _currentStep++;
                Repaint();
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (!canAdvance)
            {
                EditorGUILayout.HelpBox(step.ValidationMessage, MessageType.Warning);
            }
        }

        // Called by ReviewAndGenerateStep when the asset is ready
        public void OnAssetGenerated()
        {
            EditorUtility.DisplayDialog("Done!", "Your cultural context asset has been created in your project.", "Great!");
            Close();
        }
    }
}
