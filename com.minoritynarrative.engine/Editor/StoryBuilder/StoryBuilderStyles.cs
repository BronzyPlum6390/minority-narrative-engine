using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Shared GUIStyle constants and color palette for the story builder canvas.
    /// </summary>
    public static class StoryBuilderStyles
    {
        private static GUIStyle _sidebarStyle;
        private static GUIStyle _nodeHeaderStyle;
        private static GUIStyle _speakerLabelStyle;
        private static GUIStyle _nodePreviewStyle;

        public static GUIStyle SidebarStyle
        {
            get
            {
                if (_sidebarStyle == null)
                    _sidebarStyle = new GUIStyle(GUI.skin.box)
                    {
                        normal = { background = MakeColorTex(new Color(0.18f, 0.18f, 0.20f)) }
                    };
                return _sidebarStyle;
            }
        }

        public static GUIStyle NodeHeaderStyle
        {
            get
            {
                if (_nodeHeaderStyle == null)
                    _nodeHeaderStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = new Color(0.9f, 0.85f, 0.6f) },
                        fontStyle = FontStyle.Bold
                    };
                return _nodeHeaderStyle;
            }
        }

        public static GUIStyle SpeakerLabelStyle
        {
            get
            {
                if (_speakerLabelStyle == null)
                    _speakerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        normal = { textColor = new Color(0.7f, 0.9f, 0.8f) }
                    };
                return _speakerLabelStyle;
            }
        }

        public static GUIStyle NodePreviewStyle
        {
            get
            {
                if (_nodePreviewStyle == null)
                    _nodePreviewStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
                    {
                        normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
                    };
                return _nodePreviewStyle;
            }
        }

        public static Color GetTagColor(string tag)
        {
            return tag switch
            {
                "call_and_response" => new Color(0.4f, 0.9f, 0.6f),
                "elder_wisdom"      => new Color(0.9f, 0.8f, 0.3f),
                "signifying"        => new Color(0.9f, 0.5f, 0.9f),
                "testimony"         => new Color(0.9f, 0.4f, 0.4f),
                "story_circle"      => new Color(0.4f, 0.7f, 0.9f),
                "ancestor_voice"    => new Color(0.6f, 0.4f, 0.9f),
                "land_teaching"     => new Color(0.4f, 0.9f, 0.4f),
                "dicho"             => new Color(0.9f, 0.6f, 0.3f),
                "family_council"    => new Color(0.9f, 0.7f, 0.4f),
                "face_moment"       => new Color(0.5f, 0.8f, 0.9f),
                _                   => new Color(0.7f, 0.7f, 0.7f)
            };
        }

        private static Texture2D MakeColorTex(Color color)
        {
            var tex = new Texture2D(2, 2);
            for (int i = 0; i < 4; i++) tex.SetPixel(i % 2, i / 2, color);
            tex.Apply();
            return tex;
        }
    }
}
