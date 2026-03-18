using NUnit.Framework;
using MinorityNarrativeEngine;
using MinorityNarrativeEngine.OralTradition;
using System.Collections.Generic;

namespace MinorityNarrativeEngine.Tests
{
    public class OralTraditionProcessorTests
    {
        private StorySession _session;
        private BlackAmericanContext _context;

        [SetUp]
        public void SetUp()
        {
            _session = new StorySession();
            _context = UnityEngine.ScriptableObject.CreateInstance<BlackAmericanContext>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_context);
        }

        [Test]
        public void Registry_HasCallAndResponse_ByDefault()
        {
            Assert.IsTrue(OralTraditionProcessorRegistry.HasProcessor("call_and_response"));
        }

        [Test]
        public void Registry_HasElderWisdom_ByDefault()
        {
            Assert.IsTrue(OralTraditionProcessorRegistry.HasProcessor("elder_wisdom"));
        }

        [Test]
        public void Registry_HasSignifying_ByDefault()
        {
            Assert.IsTrue(OralTraditionProcessorRegistry.HasProcessor("signifying"));
        }

        [Test]
        public void Registry_HasTestimony_ByDefault()
        {
            Assert.IsTrue(OralTraditionProcessorRegistry.HasProcessor("testimony"));
        }

        [Test]
        public void CallAndResponse_RequiresResponse()
        {
            OralTraditionProcessorRegistry.TryGet("call_and_response", out var processor);
            var node = new StoryNode { id = "test", culturalTags = new List<string> { "call_and_response" } };
            var result = processor.Process(node, _session, _context);
            Assert.IsTrue(result.RequiresResponse);
        }

        [Test]
        public void CallAndResponse_HasResponseOptions()
        {
            OralTraditionProcessorRegistry.TryGet("call_and_response", out var processor);
            var node = new StoryNode { id = "test", culturalTags = new List<string> { "call_and_response" } };
            var result = processor.Process(node, _session, _context);
            Assert.IsNotEmpty(result.ResponseOptions);
        }

        [Test]
        public void Testimony_RequiresResponse()
        {
            OralTraditionProcessorRegistry.TryGet("testimony", out var processor);
            var node = new StoryNode { id = "test", culturalTags = new List<string> { "testimony" } };
            var result = processor.Process(node, _session, _context);
            Assert.IsTrue(result.RequiresResponse);
        }

        [Test]
        public void Signifying_DoesNotRequireResponse()
        {
            OralTraditionProcessorRegistry.TryGet("signifying", out var processor);
            var node = new StoryNode { id = "test", culturalTags = new List<string> { "signifying" } };
            var result = processor.Process(node, _session, _context);
            Assert.IsFalse(result.RequiresResponse);
        }

        [Test]
        public void RunAll_NoMatchingTags_ReturnsEmptyResult()
        {
            var node = new StoryNode { id = "test", culturalTags = new List<string> { "unknown_tag" } };
            var result = OralTraditionProcessorRegistry.RunAll(node, _session, _context);
            Assert.IsFalse(result.RequiresResponse);
            Assert.IsEmpty(result.ResponseOptions);
        }

        [Test]
        public void RunAll_MultipleTags_MergesResults()
        {
            var node = new StoryNode
            {
                id = "test",
                culturalTags = new List<string> { "call_and_response", "elder_wisdom" }
            };
            var result = OralTraditionProcessorRegistry.RunAll(node, _session, _context);
            Assert.IsTrue(result.RequiresResponse); // from call_and_response
            Assert.IsNotEmpty(result.ResponseOptions);
        }

        [Test]
        public void Register_CustomProcessor_OverridesDefault()
        {
            var custom = new TestCustomProcessor();
            OralTraditionProcessorRegistry.Register(custom);

            OralTraditionProcessorRegistry.TryGet("call_and_response", out var retrieved);
            Assert.AreEqual(custom, retrieved);

            // Restore default
            OralTraditionProcessorRegistry.Register(new DefaultCallAndResponseProcessor());
        }

        private class TestCustomProcessor : IOralTraditionProcessor
        {
            public string Role => "call_and_response";
            public ProcessorResult Process(StoryNode node, StorySession session, CulturalContextBase context)
                => new ProcessorResult { TextPrefix = "[CUSTOM] " };
        }
    }
}
