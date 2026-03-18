using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Wizard Step 2: Configure honorific tokens for the selected cultural context.
    /// </summary>
    public class HonorificsConfigStep : WizardStep
    {
        private Vector2 _scroll;
        private string _newToken = "";
        private string _newForm = "";
        private RelationshipTier _newTier = RelationshipTier.Neutral;

        public override void OnGUI(CulturalContextBase context)
        {
            if (context == null) { EditorGUILayout.HelpBox("No context loaded.", MessageType.Error); return; }

            EditorGUILayout.LabelField("Honorifics & Titles", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Honorifics are how characters address each other based on your cultural context. " +
                "In your story text you write {honorific.elder} and it gets replaced automatically.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(6);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(220));

            for (int i = 0; i < context.honorifics.Count; i++)
            {
                var h = context.honorifics[i];
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("{honorific." + h.token + "}", GUILayout.Width(160));
                EditorGUILayout.LabelField("→", GUILayout.Width(20));
                h.form = EditorGUILayout.TextField(h.form, GUILayout.Width(120));
                h.tier = (RelationshipTier)EditorGUILayout.EnumPopup(h.tier, GUILayout.Width(100));
                if (GUILayout.Button("✕", GUILayout.Width(24)))
                {
                    context.honorifics.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Add new honorific", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Token:", GUILayout.Width(50));
            _newToken = EditorGUILayout.TextField(_newToken, GUILayout.Width(120));
            EditorGUILayout.LabelField("→", GUILayout.Width(20));
            _newForm = EditorGUILayout.TextField(_newForm, GUILayout.Width(120));
            _newTier = (RelationshipTier)EditorGUILayout.EnumPopup(_newTier, GUILayout.Width(100));
            if (GUILayout.Button("Add", GUILayout.Width(50)) &&
                !string.IsNullOrEmpty(_newToken) && !string.IsNullOrEmpty(_newForm))
            {
                context.honorifics.Add(new HonorificsEntry
                    { token = _newToken.Trim(), form = _newForm.Trim(), tier = _newTier });
                _newToken = ""; _newForm = "";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "Tip: In your story JSON, write {honorific.elder} and the engine will replace it with the right term at runtime based on the speaker's relationship to the listener.",
                MessageType.Info);
        }

        public override bool Validate(CulturalContextBase context) => context != null;
        public override CulturalContextBase Apply(CulturalContextBase context) => context;
    }
}
