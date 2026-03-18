namespace MinorityNarrativeEngine.OralTradition
{
    /// <summary>
    /// Interface for custom oral tradition processors.
    /// Implement this to define how a specific cultural pattern tag
    /// transforms text and generates UI behavior.
    ///
    /// Register your processor:
    ///   OralTraditionProcessorRegistry.Register(new MyProcessor());
    ///
    /// Tag your story nodes:
    ///   "culturalTags": ["my_pattern_role"]
    ///
    /// The engine will call Process() on each tagged node before the
    /// DialogueEvent is emitted to the UI.
    /// </summary>
    public interface IOralTraditionProcessor
    {
        /// <summary>
        /// The cultural tag role this processor handles (e.g. "call_and_response").
        /// Must match a value in a story node's culturalTags array.
        /// </summary>
        string Role { get; }

        /// <summary>
        /// Processes the node text and populates the ProcessorResult with any
        /// modifications (prefix/suffix injection, response prompts, etc.).
        /// </summary>
        ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context);
    }

    /// <summary>
    /// Output of an IOralTraditionProcessor.Process() call.
    /// </summary>
    public class ProcessorResult
    {
        /// <summary>Text prepended to the node body before display.</summary>
        public string TextPrefix { get; set; } = "";

        /// <summary>Text appended to the node body before display.</summary>
        public string TextSuffix { get; set; } = "";

        /// <summary>
        /// If true, the UI should present response options to the player
        /// and wait for a response before advancing.
        /// </summary>
        public bool RequiresResponse { get; set; }

        /// <summary>Response options shown to the player (for call-and-response patterns).</summary>
        public System.Collections.Generic.List<string> ResponseOptions { get; set; }
            = new System.Collections.Generic.List<string>();

        /// <summary>
        /// Arbitrary metadata the processor wants to pass to the UI layer.
        /// Keys are processor-defined; the UI layer reads what it knows about.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> Metadata { get; set; }
            = new System.Collections.Generic.Dictionary<string, string>();

        public static ProcessorResult Empty => new ProcessorResult();
    }
}
