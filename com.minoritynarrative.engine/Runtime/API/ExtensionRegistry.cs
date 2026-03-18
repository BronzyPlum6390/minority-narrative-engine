using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine.API
{
    /// <summary>
    /// Manages registered INarrativeExtension instances and routes lifecycle events to them.
    /// Owned by NarrativeEngine and called from its internal event points.
    /// </summary>
    public class ExtensionRegistry
    {
        private readonly Dictionary<string, INarrativeExtension> _extensions
            = new Dictionary<string, INarrativeExtension>();

        private NarrativeEngine _engine;

        public ExtensionRegistry(NarrativeEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Registers an extension. If an extension with the same ID is already registered,
        /// it is replaced and a warning is logged.
        /// </summary>
        public void Register(INarrativeExtension extension)
        {
            if (_extensions.ContainsKey(extension.ExtensionId))
                Debug.LogWarning($"[MNE] Extension '{extension.ExtensionId}' replaced an existing registration.");

            _extensions[extension.ExtensionId] = extension;
            extension.OnRegistered(_engine);
            Debug.Log($"[MNE] Extension registered: {extension.ExtensionId}");
        }

        public void Unregister(string extensionId)
        {
            _extensions.Remove(extensionId);
        }

        public bool TryGet(string extensionId, out INarrativeExtension extension) =>
            _extensions.TryGetValue(extensionId, out extension);

        // -------------------------------------------------------
        // Lifecycle routing
        // -------------------------------------------------------

        public void NotifyStoryBegin(StorySession session, StoryGraph graph)
        {
            foreach (var ext in _extensions.Values)
                SafeCall(() => ext.OnStoryBegin(session, graph), ext.ExtensionId);
        }

        public void NotifyNodeEnter(StoryNode node, StorySession session)
        {
            foreach (var ext in _extensions.Values)
                SafeCall(() => ext.OnNodeEnter(node, session), ext.ExtensionId);
        }

        public void NotifyChoiceSelected(Choice choice, StorySession session)
        {
            foreach (var ext in _extensions.Values)
                SafeCall(() => ext.OnChoiceSelected(choice, session), ext.ExtensionId);
        }

        public void NotifyStoryComplete(StorySession session)
        {
            foreach (var ext in _extensions.Values)
                SafeCall(() => ext.OnStoryComplete(session), ext.ExtensionId);
        }

        private void SafeCall(System.Action action, string extensionId)
        {
            try { action(); }
            catch (System.Exception ex)
            {
                Debug.LogError($"[MNE] Extension '{extensionId}' threw an exception: {ex.Message}");
            }
        }
    }
}
