namespace MinorityNarrativeEngine.API
{
    /// <summary>
    /// Interface for custom extensions that hook into the NarrativeEngine lifecycle.
    /// Register extensions via NarrativeAPI.RegisterExtension().
    ///
    /// Extensions can react to story events, modify session state, or drive
    /// external systems (audio, animation, lighting) in response to narrative beats.
    /// </summary>
    public interface INarrativeExtension
    {
        /// <summary>Unique identifier for this extension.</summary>
        string ExtensionId { get; }

        /// <summary>Called once when the extension is registered.</summary>
        void OnRegistered(NarrativeEngine engine);

        /// <summary>Called when a new story session begins.</summary>
        void OnStoryBegin(StorySession session, StoryGraph graph);

        /// <summary>Called each time a new node is entered.</summary>
        void OnNodeEnter(StoryNode node, StorySession session);

        /// <summary>Called each time a choice is selected by the player.</summary>
        void OnChoiceSelected(Choice choice, StorySession session);

        /// <summary>Called when the story reaches an end node.</summary>
        void OnStoryComplete(StorySession session);
    }
}
