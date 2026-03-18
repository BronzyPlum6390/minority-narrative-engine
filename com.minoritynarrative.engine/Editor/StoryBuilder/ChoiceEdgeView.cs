using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Draws a directed bezier edge between two StoryNodeViews on the canvas.
    /// </summary>
    public class ChoiceEdgeView
    {
        public StoryNodeView fromNode;
        public StoryNodeView toNode;
        public int choiceIndex; // -1 for linear nextNodeId connections
        public string label;

        public ChoiceEdgeView(StoryNodeView from, StoryNodeView to, int choiceIndex, string label)
        {
            fromNode = from;
            toNode = to;
            this.choiceIndex = choiceIndex;
            this.label = label;
        }

        public void Draw(Vector2 offset, float zoom)
        {
            var fromRect = fromNode.GetWorldRect(offset, zoom);
            var toRect = toNode.GetWorldRect(offset, zoom);

            Vector3 startPos = new Vector3(fromRect.xMax, fromRect.y + fromRect.height * 0.5f, 0);
            Vector3 endPos = new Vector3(toRect.x, toRect.y + toRect.height * 0.5f, 0);

            float tangentStrength = Mathf.Max(50f, Mathf.Abs(endPos.x - startPos.x) * 0.5f);
            Vector3 startTangent = startPos + Vector3.right * tangentStrength;
            Vector3 endTangent = endPos - Vector3.right * tangentStrength;

            Color edgeColor = choiceIndex < 0
                ? new Color(0.6f, 0.6f, 0.6f, 0.7f)
                : Color.Lerp(new Color(0.2f, 0.8f, 0.4f), new Color(0.8f, 0.4f, 0.2f),
                    (choiceIndex % 3) / 2f);

            Handles.BeginGUI();
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, edgeColor, null, 2f);
            Handles.EndGUI();

            // Label at midpoint
            Vector3 mid = (startPos + endPos) / 2f;
            if (!string.IsNullOrEmpty(label))
            {
                var labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = edgeColor },
                    fontSize = Mathf.Max(9, (int)(10 * zoom))
                };
                GUI.Label(new Rect(mid.x - 60, mid.y - 10, 120, 20), label, labelStyle);
            }
        }
    }
}
