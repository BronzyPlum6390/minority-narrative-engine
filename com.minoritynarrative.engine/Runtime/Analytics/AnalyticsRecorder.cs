using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MinorityNarrativeEngine.Analytics
{
    /// <summary>
    /// Records playthrough events and generates reports.
    /// Attach to NarrativeEngine via NarrativeAPI or directly via the Inspector.
    ///
    /// Quickstart:
    ///   var recorder = new AnalyticsRecorder();
    ///   recorder.Attach(narrativeEngine);
    ///
    ///   // At end of session:
    ///   var report = recorder.GenerateReport();
    ///   recorder.ExportToFile(); // writes JSON to persistentDataPath
    ///
    /// No data leaves the device unless you explicitly call ExportToFile() or
    /// implement your own upload in OnReportGenerated.
    /// </summary>
    public class AnalyticsRecorder
    {
        private readonly List<AnalyticsEvent> _events = new List<AnalyticsEvent>();
        private string _storyTitle;
        private string _startedAtUtc;
        private bool _completed;
        private string _completedAtUtc;

        private StoryNode _currentNode;
        private DateTime _nodeEnterTime;

        public event Action<PlaythroughReport> OnReportGenerated;

        // -------------------------------------------------------
        // Attach / Detach
        // -------------------------------------------------------

        /// <summary>
        /// Wires the recorder into a NarrativeEngine's events.
        /// Call this after the engine is initialized.
        /// </summary>
        public void Attach(NarrativeEngine engine)
        {
            engine.OnStoryBegin.AddListener(OnStoryBegin);
            engine.OnDialogueEvent.AddListener(OnDialogueEvent);
            engine.OnStoryComplete.AddListener(OnStoryComplete);
        }

        // -------------------------------------------------------
        // Event handlers
        // -------------------------------------------------------

        private void OnStoryBegin()
        {
            _events.Clear();
            _startedAtUtc = DateTime.UtcNow.ToString("o");
            _completed = false;
            Record(AnalyticsEventType.StoryBegin, null, NarrativeEngine.Instance?.Session);
        }

        private void OnDialogueEvent(DialogueEvent evt)
        {
            if (evt?.sourceNode == null) return;

            // Compute dwell time for the previous node
            if (_currentNode != null)
            {
                var dwell = (long)(DateTime.UtcNow - _nodeEnterTime).TotalMilliseconds;
                var prev = _events.FindLast(e => e.type == AnalyticsEventType.NodeEnter
                                              && e.nodeId == _currentNode.id);
                if (prev != null) prev.dwellTimeMs = dwell;
            }

            _currentNode = evt.sourceNode;
            _nodeEnterTime = DateTime.UtcNow;

            var session = NarrativeEngine.Instance?.Session;
            var e = BuildEvent(AnalyticsEventType.NodeEnter, evt.sourceNode, session);
            _events.Add(e);
        }

        /// <summary>Call this from your UI when the player selects a choice.</summary>
        public void RecordChoice(ResolvedChoice choice, StoryNode sourceNode, StorySession session)
        {
            var e = BuildEvent(AnalyticsEventType.ChoiceSelected, sourceNode, session);
            e.choiceIndex        = choice.sourceIndex;
            e.choiceText         = choice.text;
            e.collectivityDelta  = choice.collectivityDelta;
            _events.Add(e);
        }

        private void OnStoryComplete()
        {
            _completed = true;
            _completedAtUtc = DateTime.UtcNow.ToString("o");
            Record(AnalyticsEventType.StoryComplete, _currentNode, NarrativeEngine.Instance?.Session);
        }

        // -------------------------------------------------------
        // Report generation
        // -------------------------------------------------------

        public PlaythroughReport GenerateReport()
        {
            var session = NarrativeEngine.Instance?.Session;
            var report = new PlaythroughReport
            {
                storyTitle           = _storyTitle,
                startedAtUtc         = _startedAtUtc,
                completedAtUtc       = _completedAtUtc,
                storyCompleted       = _completed,
                finalCollectivityScore = session?.collectivityScore ?? 0f,
                events               = new List<AnalyticsEvent>(_events)
            };

            float totalDelta = 0f;
            int choiceCount = 0;

            foreach (var e in _events)
            {
                if (e.type == AnalyticsEventType.NodeEnter)
                {
                    report.totalNodesVisited++;
                    report.visitedNodeIds.Add(e.nodeId);
                }
                else if (e.type == AnalyticsEventType.ChoiceSelected)
                {
                    report.totalChoicesMade++;
                    totalDelta += e.collectivityDelta;
                    choiceCount++;
                }
            }

            report.collectivityTrend = choiceCount > 0 ? totalDelta / choiceCount : 0f;

            if (session != null)
                report.flagsSet.AddRange(GetSessionFlags(session));

            OnReportGenerated?.Invoke(report);
            return report;
        }

        /// <summary>
        /// Exports the current playthrough report as JSON to Application.persistentDataPath.
        /// Filename: mne_report_{timestamp}.json
        /// </summary>
        public string ExportToFile()
        {
            var report = GenerateReport();
            string filename = $"mne_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            string path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllText(path, report.ToJson());
            Debug.Log($"[MNE] Analytics report exported: {path}");
            return path;
        }

        // -------------------------------------------------------
        // Helpers
        // -------------------------------------------------------

        private void Record(string type, StoryNode node, StorySession session)
        {
            _events.Add(BuildEvent(type, node, session));
        }

        private static AnalyticsEvent BuildEvent(string type, StoryNode node, StorySession session)
        {
            return new AnalyticsEvent
            {
                type                      = type,
                timestampUtc              = DateTime.UtcNow.ToString("o"),
                nodeId                    = node?.id,
                nodeType                  = node?.type,
                speakerId                 = node?.speakerId,
                culturalTags              = node?.culturalTags?.ToArray(),
                collectivityScoreAtEvent  = session?.collectivityScore ?? 0f
            };
        }

        private static IEnumerable<string> GetSessionFlags(StorySession session)
        {
            // StorySession._flags is private — expose via snapshot
            var snap = session.TakeSnapshot();
            return snap.flags ?? new System.Collections.Generic.List<string>();
        }
    }
}
