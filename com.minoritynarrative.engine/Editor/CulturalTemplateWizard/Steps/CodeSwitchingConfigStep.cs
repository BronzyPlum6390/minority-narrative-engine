using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Wizard Step 3: Configure code-switching substitution pairs.
    /// </summary>
    public class CodeSwitchingConfigStep : WizardStep
    {
        private Vector2 _scroll;
        private string _newStandard = "";
        private string _newCultural = "";
        private bool _newWholeWord = true;

        public override void OnGUI(CulturalContextBase context)
        {
            if (context == null) { EditorGUILayout.HelpBox("No context loaded.", MessageType.Error); return; }

            EditorGUILayout.LabelField("Language & Code-Switching", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Code-switching automatically replaces standard/neutral phrases with culturally specific ones at runtime. " +
                "Write your story in standard English and let the engine handle the substitution.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(6);

            context.codeSwitchingEnabled = EditorGUILayout.ToggleLeft(
                "Enable code-switching (can be toggled by players for accessibility)",
                context.codeSwitchingEnabled);
            EditorGUILayout.Space(4);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(200));
            for (int i = 0; i < context.codeSwitchingRules.Count; i++)
            {
                var rule = context.codeSwitchingRules[i];
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                rule.standard = EditorGUILayout.TextField(rule.standard, GUILayout.Width(150));
                EditorGUILayout.LabelField("→", GUILayout.Width(20));
                rule.culturalForm = EditorGUILayout.TextField(rule.culturalForm, GUILayout.Width(150));
                rule.wholeWordOnly = EditorGUILayout.ToggleLeft("Whole word", rule.wholeWordOnly, GUILayout.Width(90));
                if (GUILayout.Button("✕", GUILayout.Width(24)))
                {
                    context.codeSwitchingRules.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Add substitution", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _newStandard = EditorGUILayout.TextField(_newStandard, GUILayout.Width(150));
            EditorGUILayout.LabelField("→", GUILayout.Width(20));
            _newCultural = EditorGUILayout.TextField(_newCultural, GUILayout.Width(150));
            _newWholeWord = EditorGUILayout.ToggleLeft("Whole word", _newWholeWord, GUILayout.Width(90));
            if (GUILayout.Button("Add", GUILayout.Width(50)) &&
                !string.IsNullOrEmpty(_newStandard) && !string.IsNullOrEmpty(_newCultural))
            {
                context.codeSwitchingRules.Add(new CodeSwitchEntry
                    { standard = _newStandard.Trim(), culturalForm = _newCultural.Trim(), wholeWordOnly = _newWholeWord });
                _newStandard = ""; _newCultural = "";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "Example: write 'you all' in your story → players with code-switching enabled see \"y'all\".\n" +
                "\"Whole word\" prevents partial matches (e.g. 'family' won't match inside 'familiarity').",
                MessageType.Info);
        }

        public override bool Validate(CulturalContextBase context) => context != null;
        public override CulturalContextBase Apply(CulturalContextBase context) => context;
    }
}
