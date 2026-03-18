using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Wizard Step 1: Choose a cultural context base type with plain-language descriptions.
    /// </summary>
    public class SelectContextStep : WizardStep
    {
        private static readonly ContextOption[] Options = {
            new ContextOption(
                "Black American",
                "black_american",
                "For stories rooted in Black American culture. Includes AAVE language patterns, kinship honorifics (OG, Auntie, Big Homie), call-and-response oral tradition, and strong community-first choice framing.",
                typeof(BlackAmericanContext)),

            new ContextOption(
                "Indigenous",
                "indigenous",
                "For stories centered on Indigenous communities and land relationships. Includes elder/ancestor honorifics, story circle oral tradition, land-relational language, and the highest collectivity weighting of any context. Note: Indigenous cultures are highly diverse — customize for your specific nation.",
                typeof(IndigenousContext)),

            new ContextOption(
                "Southeast Asian",
                "southeast_asian",
                "For stories from Southeast Asian cultures (Filipino, Vietnamese, Thai, and others). Includes hierarchical honorifics (Kuya, Ate, Lola, Phi), face-saving indirect speech, and strong family-collective weighting. Customize for your specific culture.",
                typeof(SouthEastAsianContext)),

            new ContextOption(
                "Latinx",
                "latinx",
                "For stories from Latinx cultures. Includes familismo honorifics (Abuela, Don/Doña, Compadre), Spanglish code-switching, dichos (proverbs), and communal weighting. Customize for your specific region and community.",
                typeof(LatinxContext))
        };

        private int _selectedIndex = -1;
        private string _assetName = "My_Cultural_Context";

        public override void OnGUI(CulturalContextBase context)
        {
            EditorGUILayout.LabelField("Choose Your Cultural Context", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Select the cultural foundation for your story. You'll be able to customize everything on the next pages.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(8);

            for (int i = 0; i < Options.Length; i++)
            {
                bool isSelected = _selectedIndex == i;
                var style = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10)
                };
                GUI.color = isSelected ? new Color(0.4f, 0.8f, 0.6f, 0.3f) : Color.white;
                EditorGUILayout.BeginVertical(style);
                GUI.color = Color.white;

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(isSelected, Options[i].displayName, EditorStyles.boldLabel))
                    _selectedIndex = i;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(Options[i].description, EditorStyles.wordWrappedLabel);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.Space(8);
            _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
            EditorGUILayout.HelpBox("This will be the filename of the ScriptableObject asset created in your project.", MessageType.None);
        }

        public override bool Validate(CulturalContextBase context) =>
            _selectedIndex >= 0 && !string.IsNullOrEmpty(_assetName);

        public override string ValidationMessage => "Please select a cultural context to continue.";

        public override CulturalContextBase Apply(CulturalContextBase _)
        {
            var option = Options[_selectedIndex];
            var instance = ScriptableObject.CreateInstance(option.contextType) as CulturalContextBase;
            instance.name = _assetName;
            // Reset() is called by Unity's CreateInstance, which sets the defaults defined in each context class
            return instance;
        }

        private class ContextOption
        {
            public string displayName;
            public string key;
            public string description;
            public System.Type contextType;

            public ContextOption(string name, string key, string desc, System.Type type)
            {
                displayName = name;
                this.key = key;
                description = desc;
                contextType = type;
            }
        }
    }
}
