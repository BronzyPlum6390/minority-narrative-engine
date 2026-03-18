using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Wizard Step 5: Show a summary and generate the ScriptableObject asset.
    /// </summary>
    public class ReviewAndGenerateStep : WizardStep
    {
        private string _savePath = "Assets/";
        private Vector2 _scroll;

        public override void OnGUI(CulturalContextBase context)
        {
            if (context == null) { EditorGUILayout.HelpBox("No context to review.", MessageType.Error); return; }

            EditorGUILayout.LabelField("Review & Generate", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Here's a summary of your cultural context configuration.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(8);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(280));

            EditorGUILayout.LabelField($"Context:  {context.displayName} ({context.contextKey})", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Collectivity Weight:  {context.collectivityWeight:P0}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Show Community Impact:  {context.showCommunityImpact}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Code-switching:  {(context.codeSwitchingEnabled ? "Enabled" : "Disabled")}  ({context.codeSwitchingRules.Count} rules)", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Honorifics:  {context.honorifics.Count} entries", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Oral Tradition Patterns:  {context.oralTraditionPatterns.Count}", EditorStyles.miniLabel);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Honorifics", EditorStyles.boldLabel);
            foreach (var h in context.honorifics)
                EditorGUILayout.LabelField($"  {{honorific.{h.token}}} → {h.form}  [{h.tier}]", EditorStyles.miniLabel);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Code-Switching Rules", EditorStyles.boldLabel);
            foreach (var rule in context.codeSwitchingRules)
                EditorGUILayout.LabelField($"  \"{rule.standard}\" → \"{rule.culturalForm}\"", EditorStyles.miniLabel);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(8);
            _savePath = EditorGUILayout.TextField("Save to folder", _savePath);
            EditorGUILayout.HelpBox("The .asset file will be saved to the path above inside your Unity project.", MessageType.None);
        }

        public override bool Validate(CulturalContextBase context) => context != null && !string.IsNullOrEmpty(_savePath);

        public override CulturalContextBase Apply(CulturalContextBase context)
        {
            string fullPath = $"{_savePath.TrimEnd('/')}/{context.name}.asset";
            AssetDatabase.CreateAsset(context, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = context;
            Debug.Log($"[MNE] Cultural context asset created at: {fullPath}");
            wizard?.OnAssetGenerated();
            return context;
        }
    }
}
