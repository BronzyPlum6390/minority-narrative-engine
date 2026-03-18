using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class StoryLoaderTests
    {
        private const string ValidStoryJson = @"{
            ""title"": ""Test Story"",
            ""culturalContext"": ""black_american"",
            ""startNodeId"": ""intro"",
            ""nodes"": [
                {
                    ""id"": ""intro"",
                    ""type"": ""dialogue"",
                    ""speakerId"": ""isaiah"",
                    ""text"": ""This is a test."",
                    ""choices"": [
                        {
                            ""text"": ""Go forward"",
                            ""targetNodeId"": ""end_node"",
                            ""collectivityDelta"": 0.5
                        }
                    ]
                },
                {
                    ""id"": ""end_node"",
                    ""type"": ""end"",
                    ""text"": ""The end.""
                }
            ]
        }";

        [Test]
        public void LoadFromJson_ValidJson_ReturnsStoryGraph()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            Assert.IsNotNull(graph);
            Assert.AreEqual("Test Story", graph.title);
        }

        [Test]
        public void LoadFromJson_ValidJson_SetsStartNodeId()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            Assert.AreEqual("intro", graph.startNodeId);
        }

        [Test]
        public void LoadFromJson_ValidJson_LoadsAllNodes()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            Assert.AreEqual(2, graph.nodes.Count);
        }

        [Test]
        public void LoadFromJson_ValidJson_BuildsNodeLookup()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            Assert.IsNotNull(graph.GetNode("intro"));
            Assert.IsNotNull(graph.GetNode("end_node"));
        }

        [Test]
        public void LoadFromJson_ValidJson_ChoicesDeserializeCorrectly()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            var intro = graph.GetNode("intro");
            Assert.AreEqual(1, intro.choices.Count);
            Assert.AreEqual("Go forward", intro.choices[0].text);
            Assert.AreEqual("end_node", intro.choices[0].targetNodeId);
            Assert.AreEqual(0.5f, intro.choices[0].collectivityDelta, 0.001f);
        }

        [Test]
        public void LoadFromJson_MissingStartNodeId_ThrowsValidationException()
        {
            string json = @"{
                ""title"": ""Bad Story"",
                ""nodes"": [{ ""id"": ""a"", ""text"": ""hi"" }]
            }";
            Assert.Throws<StoryValidationException>(() => StoryLoader.LoadFromJson(json));
        }

        [Test]
        public void LoadFromJson_EmptyNodes_ThrowsValidationException()
        {
            string json = @"{
                ""title"": ""Bad Story"",
                ""startNodeId"": ""intro"",
                ""nodes"": []
            }";
            Assert.Throws<StoryValidationException>(() => StoryLoader.LoadFromJson(json));
        }

        [Test]
        public void LoadFromJson_StartNodeIdNotInNodes_ThrowsValidationException()
        {
            string json = @"{
                ""title"": ""Bad Story"",
                ""startNodeId"": ""missing_node"",
                ""nodes"": [{ ""id"": ""actual_node"", ""text"": ""hi"" }]
            }";
            Assert.Throws<StoryValidationException>(() => StoryLoader.LoadFromJson(json));
        }

        [Test]
        public void GetNode_ExistingId_ReturnsNode()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            var node = graph.GetNode("intro");
            Assert.IsNotNull(node);
            Assert.AreEqual("intro", node.id);
        }

        [Test]
        public void GetNode_NonExistingId_ReturnsNull()
        {
            var graph = StoryLoader.LoadFromJson(ValidStoryJson);
            Assert.IsNull(graph.GetNode("does_not_exist"));
        }
    }
}
