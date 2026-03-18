using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Visual representation of a StoryNode on the story builder canvas.
    /// </summary>
    public class StoryNodeView
    {
        public StoryNode nodeData;
        public Vector2 position;

        private static readonly Vector2 NodeSize = new Vector2(200, 120);

        public StoryNodeView(StoryNode node, Vector2 initialPosition)
        {
            nodeData = node;
            position = initialPosition;
        }

        public Rect GetWorldRect(Vector2 offset, float zoom)
        {
            return new Rect(
                position.x * zoom + offset.x,
                position.y * zoom + offset.y,
                NodeSize.x * zoom,
                NodeSize.y * zoom);
        }

        public void Draw(Vector2 offset, float zoom, bool selected)
        {
            var rect = GetWorldRect(offset, zoom);

            // Node background
            Color nodeColor = GetNodeColor(nodeData.type, nodeData.collectiveFrame);
            if (selected) nodeColor = Color.Lerp(nodeColor, Color.white, 0.3f);

            var bgStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeColorTexture(2, 2, nodeColor) }
            };
            GUI.Box(rect, GUIContent.none, bgStyle);

            if (selected)
            {
                var borderRect = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
                Handles.BeginGUI();
                Handles.color = Color.yellow;
                Handles.DrawSolidRectangleWithOutline(borderRect, Color.clear, Color.yellow);
                Handles.EndGUI();
            }

            // Content
            float padding = 6 * zoom;
            var contentRect = new Rect(rect.x + padding, rect.y + padding,
                rect.width - padding * 2, rect.height - padding * 2);

            GUILayout.BeginArea(contentRect);

            // Type badge + ID
            EditorGUILayout.LabelField($"[{nodeData.type.ToUpper()}]  {nodeData.id}",
                StoryBuilderStyles.NodeHeaderStyle);

            // Speaker
            if (!string.IsNullOrEmpty(nodeData.speakerId))
                EditorGUILayout.LabelField(nodeData.speakerId, StoryBuilderStyles.SpeakerLabelStyle);

            // Text preview
            string preview = nodeData.text ?? "";
            if (preview.Length > 60) preview = preview.Substring(0, 57) + "...";
            EditorGUILayout.LabelField(preview, StoryBuilderStyles.NodePreviewStyle);

            // Cultural tag badges
            if (nodeData.culturalTags != null && nodeData.culturalTags.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var tag in nodeData.culturalTags)
                {
                    var tagStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = StoryBuilderStyles.GetTagColor(tag) }
                    };
                    GUILayout.Label($"#{tag}", tagStyle);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndArea();

            // Drag
            HandleDrag(rect, offset, zoom);
        }

        private void HandleDrag(Rect rect, Vector2 offset, float zoom)
        {
            var e = Event.current;
            if (e.type == EventType.MouseDrag && e.button == 0 && rect.Contains(e.mousePosition))
            {
                position += e.delta / zoom;
                GUI.changed = true;
                e.Use();
            }
        }

        private static Color GetNodeColor(string type, string collectiveFrame)
        {
            Color baseColor = type switch
            {
                "dialogue"      => new Color(0.20f, 0.26f, 0.38f),
                "narration"     => new Color(0.18f, 0.28f, 0.22f),
                "choice_prompt" => new Color(0.35f, 0.24f, 0.14f),
                "community_event" => new Color(0.28f, 0.22f, 0.38f),
                "end"           => new Color(0.38f, 0.18f, 0.18f),
                _               => new Color(0.25f, 0.25f, 0.25f)
            };
            return baseColor;
        }

        private static Texture2D MakeColorTexture(int width, int height, Color color)
        {
            var tex = new Texture2D(width, height);
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
