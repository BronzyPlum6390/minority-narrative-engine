using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Drives dialogue playback: advances nodes, evaluates conditions, resolves text,
    /// and emits DialogueEvents for the UI layer to render.
    /// </summary>
    public class DialogueSystem
    {
        private StoryGraph _graph;
        private StorySession _session;
        private CulturalContextBase _context;
        private CharacterRegistry _characters;

        public void Initialize(
            StoryGraph graph,
            StorySession session,
            CulturalContextBase context,
            CharacterRegistry characters)
        {
            _graph = graph;
            _session = session;
            _context = context;
            _characters = characters;
        }

        /// <summary>
        /// Advances to the given node, evaluates conditions on its choices, resolves all text,
        /// and returns a DialogueEvent for the UI.
        /// </summary>
        public DialogueEvent StepToNode(string nodeId)
        {
            var node = _graph.GetNode(nodeId);
            if (node == null)
            {
                Debug.LogError($"[MNE] DialogueSystem: node '{nodeId}' not found in graph.");
                return null;
            }

            // Fire entry triggers
            foreach (var trigger in node.triggers)
                trigger.Apply(_session);

            // Mark visited
            _session.MarkVisited(nodeId);
            _session.currentNodeId = nodeId;

            // Resolve text
            string resolved = ResolveText(node.text, node.speakerId);

            // Apply oral tradition patterns
            resolved = ApplyOralTradition(resolved, node, out bool isCallAndResponse, out List<string> suggestedResponses);

            // Build resolved choices
            var resolvedChoices = BuildChoices(node);

            // Determine speaker display name
            string speakerName = "";
            string portraitId = "";
            if (!string.IsNullOrEmpty(node.speakerId) && _characters != null)
            {
                var profile = _characters.Get(node.speakerId);
                if (profile != null)
                {
                    speakerName = profile.displayName;
                    portraitId = profile.portraitId;
                }
                else
                {
                    speakerName = node.speakerId;
                }
            }

            return new DialogueEvent
            {
                sourceNode = node,
                speakerId = node.speakerId,
                speakerDisplayName = speakerName,
                resolvedText = resolved,
                choices = resolvedChoices,
                awaitingInput = resolvedChoices.Count > 0 || node.displayDuration < 0f,
                portraitId = portraitId,
                culturalTags = node.culturalTags,
                collectiveFrame = node.collectiveFrame,
                isCallAndResponse = isCallAndResponse,
                suggestedResponses = suggestedResponses
            };
        }

        /// <summary>
        /// Called when the player selects a choice. Applies choice triggers, updates
        /// collectivity score, and returns the next DialogueEvent.
        /// Returns null if the story has ended.
        /// </summary>
        public DialogueEvent SelectChoice(ResolvedChoice choice)
        {
            if (choice == null) return null;

            // Apply choice triggers
            foreach (var trigger in choice.sourceChoice.triggers)
                trigger.Apply(_session);

            // Track collectivity
            _session.ApplyCollectivityDelta(choice.collectivityDelta);
            _session.choiceHistory.Add(choice.sourceChoice.targetNodeId);

            if (string.IsNullOrEmpty(choice.sourceChoice.targetNodeId))
                return null; // story end

            return StepToNode(choice.sourceChoice.targetNodeId);
        }

        /// <summary>
        /// Advances to the next node in linear flow (no choices).
        /// Returns null at story end.
        /// </summary>
        public DialogueEvent Advance()
        {
            var currentNode = _graph.GetNode(_session.currentNodeId);
            if (currentNode == null || string.IsNullOrEmpty(currentNode.nextNodeId))
                return null;
            return StepToNode(currentNode.nextNodeId);
        }

        // -------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------

        private string ResolveText(string raw, string speakerId)
        {
            if (string.IsNullOrEmpty(raw)) return raw;

            // 1. Honorifics resolution
            string text = HonorificsResolver.Resolve(raw, _context, _session, speakerId);

            // 2. Code-switching
            text = CodeSwitchingProcessor.Process(text, _context);

            return text;
        }

        private string ApplyOralTradition(
            string text,
            StoryNode node,
            out bool isCallAndResponse,
            out List<string> suggestedResponses)
        {
            isCallAndResponse = false;
            suggestedResponses = null;

            if (_context == null || node.culturalTags == null) return text;

            foreach (var tag in node.culturalTags)
            {
                var pattern = _context.GetPattern(tag);
                if (pattern == null) continue;

                if (!string.IsNullOrEmpty(pattern.textPrefix))
                    text = pattern.textPrefix + text;
                if (!string.IsNullOrEmpty(pattern.textSuffix))
                    text = text + pattern.textSuffix;

                if (pattern.requiresResponse)
                {
                    isCallAndResponse = true;
                    suggestedResponses = pattern.suggestedResponses;
                }
            }
            return text;
        }

        private List<ResolvedChoice> BuildChoices(StoryNode node)
        {
            var result = new List<ResolvedChoice>();
            for (int i = 0; i < node.choices.Count; i++)
            {
                var choice = node.choices[i];

                // Evaluate conditions
                bool available = true;
                foreach (var cond in choice.conditions)
                {
                    if (!cond.Evaluate(_session))
                    {
                        available = false;
                        break;
                    }
                }
                if (!available) continue;

                result.Add(new ResolvedChoice
                {
                    sourceIndex = i,
                    text = ResolveText(choice.text, node.speakerId),
                    communityImpact = _context?.showCommunityImpact == true ? choice.communityImpact : null,
                    individualImpact = choice.individualImpact,
                    collectivityDelta = choice.collectivityDelta,
                    sourceChoice = choice
                });
            }
            return result;
        }
    }
}
