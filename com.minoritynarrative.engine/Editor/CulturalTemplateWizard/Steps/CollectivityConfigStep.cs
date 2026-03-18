using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Wizard Step 4: Configure individual vs collective weighting with a plain-language slider.
    /// </summary>
    public class CollectivityConfigStep : WizardStep
    {
        public override void OnGUI(CulturalContextBase context)
        {
            if (context == null) { EditorGUILayout.HelpBox("No context loaded.", MessageType.Error); return; }

            EditorGUILayout.LabelField("Community & Individual Balance", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "In your story, how much weight does the community's wellbeing carry compared to the individual character's desires?",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Individual-focused", GUILayout.Width(140));
            context.collectivityWeight = EditorGUILayout.Slider(context.collectivityWeight, 0f, 1f);
            EditorGUILayout.LabelField("Community-focused", GUILayout.Width(140));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);
            string desc = context.collectivityWeight switch
            {
                >= 0.85f => "Very high collectivity — community outcomes are front and center. Individual desires take a back seat to the group's wellbeing.",
                >= 0.65f => "Strong collectivity — the community's needs are important and visible, but personal stakes still matter.",
                >= 0.45f => "Balanced — individual and community outcomes are weighted roughly equally. Neither dominates.",
                >= 0.25f => "Individual-leaning — personal stakes drive most decisions. Community impact is present but secondary.",
                _        => "Strongly individual — the story centers a single character's choices with minimal community framing."
            };
            EditorGUILayout.HelpBox(desc, MessageType.None);

            EditorGUILayout.Space(10);
            context.showCommunityImpact = EditorGUILayout.ToggleLeft(
                "Show community impact text alongside player choices",
                context.showCommunityImpact);

            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "When enabled, each choice button shows a one-line description of how that choice affects the community (in addition to the personal impact).",
                MessageType.Info);
        }

        public override bool Validate(CulturalContextBase context) => context != null;

        public override CulturalContextBase Apply(CulturalContextBase context) => context;
    }
}
