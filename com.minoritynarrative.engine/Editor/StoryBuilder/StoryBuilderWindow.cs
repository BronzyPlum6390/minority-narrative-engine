using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MinorityNarrativeEngine.Editor
{
    /// <summary>
    /// Visual story node graph editor for non-technical authors.
    /// Open via: Window > Minority Narrative > Story Builder
    ///
    /// Features:
    /// - Drag-and-drop node canvas
    /// - Add dialogue, narration, and choice nodes with one click
    /// - Cultural tag badges with color coding
    /// - Collectivity framing indicator per node
    /// - Export to .story.json
    /// - Import existing .story.json for editing
    /// </summary>
    public class StoryBuilderWindow : EditorWindow
    {
        // -------------------------------------------------------
        // Canvas state
        // -------------------------------------------------------

        private List<StoryNodeView> _nodeViews = new List<StoryNodeView>();
        private List<ChoiceEdgeView> _edgeViews = new List<ChoiceEdgeView>();
        private Vector2 _canvasOffset = Vector2.zero;
        private float _canvasZoom = 1f;
        private bool _isPanning;
        private Vector2 _panStart;
        private StoryNodeView _selectedNode;
        private StoryNodeView _connectingFrom;
        private int _connectingChoiceIndex;

        // -------------------------------------------------------
        // Sidebar
        // -------------------------------------------------------

        private Vector2 _sidebarScroll;
        private const float SidebarWidth = 280f;

        // -------------------------------------------------------
        // Story metadata
        // -------------------------------------------------------

        private string _storyTitle = "My Story";
        private string _storyAuthor = "";
        private string _culturalContext = "black_american";
        private string _startNodeId = "";

        private static readonly string[] ContextOptions =
            { "black_american", "indigenous", "southeast_asian", "latinx" };

        private static readonly string[] ContextLabels =
            { "Black American", "Indigenous", "Southeast Asian", "Latinx" };

        // -------------------------------------------------------
        // Open / Menu
        // -------------------------------------------------------

        [MenuItem("Window/Minority Narrative/Story Builder")]
        public static void Open()
        {
            var window = GetWindow<StoryBuilderWindow>("Story Builder");
            window.minSize = new Vector2(900, 600);
            window.Show();
        }

        [MenuItem("Window/Minority Narrative/New Story")]
        public static void NewStory()
        {
            var window = GetWindow<StoryBuilderWindow>("Story Builder");
            window.ClearCanvas();
            window.Show();
        }

        // -------------------------------------------------------
        // GUI
        // -------------------------------------------------------

        private void OnGUI()
        {
            DrawToolbar();

            var canvasRect = new Rect(SidebarWidth, EditorGUIUtility.singleLineHeight + 4,
                position.width - SidebarWidth, position.height - EditorGUIUtility.singleLineHeight - 4);
            var sidebarRect = new Rect(0, EditorGUIUtility.singleLineHeight + 4,
                SidebarWidth, position.height - EditorGUIUtility.singleLineHeight - 4);

            DrawSidebar(sidebarRect);
            DrawCanvas(canvasRect);

            // Repaint on mouse move for smooth hover states
            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }

        // -------------------------------------------------------
        // Toolbar
        // -------------------------------------------------------

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("+ Dialogue", EditorStyles.toolbarButton, GUILayout.Width(90)))
                AddNode("dialogue");
            if (GUILayout.Button("+ Narration", EditorStyles.toolbarButton, GUILayout.Width(90)))
                AddNode("narration");
            if (GUILayout.Button("+ Choice", EditorStyles.toolbarButton, GUILayout.Width(90)))
                AddNode("choice_prompt");
            if (GUILayout.Button("+ End", EditorStyles.toolbarButton, GUILayout.Width(60)))
                AddNode("end");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton, GUILayout.Width(90)))
                ImportStory();
            if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton, GUILayout.Width(90)))
                ExportStory();

            EditorGUILayout.EndHorizontal();
        }

        // -------------------------------------------------------
        // Sidebar
        // -------------------------------------------------------

        private void DrawSidebar(Rect rect)
        {
            GUI.Box(rect, GUIContent.none, StoryBuilderStyles.SidebarStyle);
            GUILayout.BeginArea(rect);
            _sidebarScroll = EditorGUILayout.BeginScrollView(_sidebarScroll);

            // Story metadata
            EditorGUILayout.LabelField("Story", EditorStyles.boldLabel);
            _storyTitle = EditorGUILayout.TextField("Title", _storyTitle);
            _storyAuthor = EditorGUILayout.TextField("Author", _storyAuthor);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Cultural Context", EditorStyles.boldLabel);
            int contextIdx = System.Array.IndexOf(ContextOptions, _culturalContext);
            if (contextIdx < 0) contextIdx = 0;
            contextIdx = EditorGUILayout.Popup(contextIdx, ContextLabels);
            _culturalContext = ContextOptions[contextIdx];

            if (GUILayout.Button("Open Template Wizard"))
                CulturalTemplateWizard.Open();

            EditorGUILayout.Space(8);

            // Selected node details
            if (_selectedNode != null)
            {
                DrawNodeInspector(_selectedNode);
            }
            else
            {
                EditorGUILayout.HelpBox("Click a node to inspect and edit it.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawNodeInspector(StoryNodeView view)
        {
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

            view.nodeData.id = EditorGUILayout.TextField("ID", view.nodeData.id);
            view.nodeData.speakerId = EditorGUILayout.TextField("Speaker ID", view.nodeData.speakerId);

            EditorGUILayout.LabelField("Text");
            view.nodeData.text = EditorGUILayout.TextArea(view.nodeData.text, GUILayout.Height(80));

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Collectivity Frame", EditorStyles.miniLabel);
            string[] frameOptions = { "none", "individual", "collective", "both" };
            int frameIdx = System.Array.IndexOf(frameOptions, view.nodeData.collectiveFrame);
            if (frameIdx < 0) frameIdx = 0;
            frameIdx = EditorGUILayout.Popup(frameIdx, frameOptions);
            view.nodeData.collectiveFrame = frameOptions[frameIdx];

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Next Node ID (linear flow)", EditorStyles.miniLabel);
            view.nodeData.nextNodeId = EditorGUILayout.TextField(view.nodeData.nextNodeId ?? "");

            EditorGUILayout.Space(8);

            // Choices
            EditorGUILayout.LabelField($"Choices ({view.nodeData.choices.Count})", EditorStyles.boldLabel);
            for (int i = 0; i < view.nodeData.choices.Count; i++)
            {
                var choice = view.nodeData.choices[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                choice.text = EditorGUILayout.TextField("Text", choice.text);
                choice.targetNodeId = EditorGUILayout.TextField("→ Target Node", choice.targetNodeId);
                choice.communityImpact = EditorGUILayout.TextField("Community Impact", choice.communityImpact ?? "");
                choice.individualImpact = EditorGUILayout.TextField("Individual Impact", choice.individualImpact ?? "");
                choice.collectivityDelta = EditorGUILayout.Slider("Collectivity Δ", choice.collectivityDelta, -1f, 1f);
                if (GUILayout.Button("Remove Choice", GUILayout.Height(18)))
                {
                    view.nodeData.choices.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("+ Add Choice"))
                view.nodeData.choices.Add(new Choice { text = "New choice", targetNodeId = "" });

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Delete Node", GUILayout.Height(24)))
            {
                RemoveNode(view);
                _selectedNode = null;
            }
        }

        // -------------------------------------------------------
        // Canvas
        // -------------------------------------------------------

        private void DrawCanvas(Rect rect)
        {
            GUI.BeginGroup(rect);

            // Background grid
            DrawGrid(rect);

            // Pan/zoom input
            HandleCanvasInput(rect);

            // Draw edges first (behind nodes)
            foreach (var edge in _edgeViews)
                edge.Draw(_canvasOffset, _canvasZoom);

            // Draw nodes
            foreach (var nodeView in _nodeViews)
            {
                bool isSelected = nodeView == _selectedNode;
                nodeView.Draw(_canvasOffset, _canvasZoom, isSelected);
            }

            // Node selection + drag
            HandleNodeInteraction(rect);

            GUI.EndGroup();
        }

        private void DrawGrid(Rect rect)
        {
            float gridSpacing = 20f * _canvasZoom;
            int widthDivs = Mathf.CeilToInt(rect.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(rect.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);

            float xOffset = _canvasOffset.x % gridSpacing;
            float yOffset = _canvasOffset.y % gridSpacing;

            for (int i = 0; i <= widthDivs; i++)
            {
                float x = gridSpacing * i + xOffset;
                Handles.DrawLine(new Vector3(x, 0), new Vector3(x, rect.height));
            }
            for (int j = 0; j <= heightDivs; j++)
            {
                float y = gridSpacing * j + yOffset;
                Handles.DrawLine(new Vector3(0, y), new Vector3(rect.width, y));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void HandleCanvasInput(Rect rect)
        {
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 2)
            {
                _isPanning = true;
                _panStart = e.mousePosition - _canvasOffset;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _isPanning)
            {
                _canvasOffset = e.mousePosition - _panStart;
                Repaint();
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == 2)
            {
                _isPanning = false;
            }
            else if (e.type == EventType.ScrollWheel)
            {
                _canvasZoom = Mathf.Clamp(_canvasZoom - e.delta.y * 0.05f, 0.3f, 2.5f);
                Repaint();
                e.Use();
            }
        }

        private void HandleNodeInteraction(Rect canvasRect)
        {
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                _selectedNode = null;
                foreach (var nodeView in _nodeViews)
                {
                    if (nodeView.GetWorldRect(_canvasOffset, _canvasZoom).Contains(e.mousePosition))
                    {
                        _selectedNode = nodeView;
                        break;
                    }
                }
                Repaint();
            }
        }

        // -------------------------------------------------------
        // Node management
        // -------------------------------------------------------

        private void AddNode(string type)
        {
            var node = new StoryNode
            {
                id = $"node_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}",
                type = type,
                text = type == "end" ? "The end." : "Enter text here..."
            };
            var view = new StoryNodeView(node,
                new Vector2(200 + _nodeViews.Count * 20, 200 + _nodeViews.Count * 20));
            _nodeViews.Add(view);
            _selectedNode = view;
            Repaint();
        }

        private void RemoveNode(StoryNodeView view)
        {
            _nodeViews.Remove(view);
            _edgeViews.RemoveAll(e => e.fromNode == view || e.toNode == view);
            Repaint();
        }

        private void ClearCanvas()
        {
            _nodeViews.Clear();
            _edgeViews.Clear();
            _selectedNode = null;
            _storyTitle = "My Story";
            _storyAuthor = "";
            _canvasOffset = Vector2.zero;
            Repaint();
        }

        // -------------------------------------------------------
        // Import / Export
        // -------------------------------------------------------

        private void ImportStory()
        {
            string path = EditorUtility.OpenFilePanel("Import Story JSON", Application.dataPath, "json");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                string json = System.IO.File.ReadAllText(path);
                var graph = StoryLoader.LoadFromJson(json, System.IO.Path.GetFileName(path));
                LoadGraphIntoCanvas(graph);
                Debug.Log($"[MNE] Story Builder: imported '{graph.title}'");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Import Failed", ex.Message, "OK");
            }
        }

        private void ExportStory()
        {
            string path = EditorUtility.SaveFilePanel("Export Story JSON",
                Application.dataPath, _storyTitle.Replace(" ", "_") + ".story", "json");
            if (string.IsNullOrEmpty(path)) return;

            var serializer = new StoryBuilderSerializer();
            string json = serializer.Serialize(BuildGraphFromCanvas());
            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            Debug.Log($"[MNE] Story Builder: exported to {path}");
        }

        private StoryGraph BuildGraphFromCanvas()
        {
            var graph = new StoryGraph
            {
                title = _storyTitle,
                author = _storyAuthor,
                culturalContext = _culturalContext,
                startNodeId = _startNodeId
            };

            if (string.IsNullOrEmpty(graph.startNodeId) && _nodeViews.Count > 0)
                graph.startNodeId = _nodeViews[0].nodeData.id;

            foreach (var view in _nodeViews)
                graph.nodes.Add(view.nodeData);

            return graph;
        }

        private void LoadGraphIntoCanvas(StoryGraph graph)
        {
            ClearCanvas();
            _storyTitle = graph.title ?? "";
            _storyAuthor = graph.author ?? "";
            _culturalContext = graph.culturalContext ?? "black_american";
            _startNodeId = graph.startNodeId ?? "";

            float x = 50, y = 50;
            foreach (var node in graph.nodes)
            {
                var view = new StoryNodeView(node, new Vector2(x, y));
                _nodeViews.Add(view);
                x += 240;
                if (x > 1200) { x = 50; y += 180; }
            }

            // Rebuild edges
            RebuildEdges();
        }

        private void RebuildEdges()
        {
            _edgeViews.Clear();
            var lookup = new Dictionary<string, StoryNodeView>();
            foreach (var v in _nodeViews) lookup[v.nodeData.id] = v;

            foreach (var view in _nodeViews)
            {
                for (int i = 0; i < view.nodeData.choices.Count; i++)
                {
                    var choice = view.nodeData.choices[i];
                    if (!string.IsNullOrEmpty(choice.targetNodeId) &&
                        lookup.TryGetValue(choice.targetNodeId, out var target))
                    {
                        _edgeViews.Add(new ChoiceEdgeView(view, target, i, choice.text));
                    }
                }

                if (!string.IsNullOrEmpty(view.nodeData.nextNodeId) &&
                    lookup.TryGetValue(view.nodeData.nextNodeId, out var nextTarget))
                {
                    _edgeViews.Add(new ChoiceEdgeView(view, nextTarget, -1, "→"));
                }
            }
        }
    }
}
