using NUnit.Framework;
using MinorityNarrativeEngine;
using MinorityNarrativeEngine.Localization;

namespace MinorityNarrativeEngine.Tests
{
    public class LocalizationManagerTests
    {
        private LocalizationManager _manager;

        private const string SpanishTableJson = @"{
            ""languageCode"": ""es"",
            ""languageName"": ""\u00c9spanol"",
            ""entries"": [
                { ""key"": ""intro::text"",    ""value"": ""Hola, mundo."" },
                { ""key"": ""intro::choice_0"", ""value"": ""S\u00ed, vamos."" },
                { ""key"": ""intro::community_0"", ""value"": ""La comunidad mejora."" }
            ]
        }";

        [SetUp]
        public void SetUp()
        {
            _manager = UnityEngine.ScriptableObject.CreateInstance<LocalizationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_manager);
        }

        [Test]
        public void SetLanguage_UnknownLanguage_ReturnsFalse()
        {
            Assert.IsFalse(_manager.SetLanguage("klingon"));
        }

        [Test]
        public void TranslateNodeText_NoActiveLanguage_ReturnsOriginal()
        {
            string original = "Hello, world.";
            string result = _manager.TranslateNodeText("intro", original);
            Assert.AreEqual(original, result);
        }

        [Test]
        public void TranslateChoiceText_NoActiveLanguage_ReturnsOriginal()
        {
            string original = "Yes, let's go.";
            string result = _manager.TranslateChoiceText("intro", 0, original);
            Assert.AreEqual(original, result);
        }

        [Test]
        public void GenerateStarterTable_CreatesEntriesForAllNodes()
        {
            var graph = StoryLoader.LoadFromJson(@"{
                ""title"": ""Test"",
                ""startNodeId"": ""a"",
                ""nodes"": [
                    { ""id"": ""a"", ""text"": ""Hello"", ""choices"": [
                        { ""text"": ""Go"", ""targetNodeId"": ""b"",
                          ""communityImpact"": ""Good for community"",
                          ""individualImpact"": ""Good for self"" }
                    ]},
                    { ""id"": ""b"", ""type"": ""end"", ""text"": ""The end."" }
                ]
            }");

            string json = LocalizationManager.GenerateStarterTable(graph, "es", "Español");
            Assert.IsNotNull(json);
            StringAssert.Contains("a::text", json);
            StringAssert.Contains("a::choice_0", json);
            StringAssert.Contains("a::community_0", json);
            StringAssert.Contains("a::individual_0", json);
            StringAssert.Contains("b::text", json);
        }

        [Test]
        public void GenerateStarterTable_PreservesOriginalText()
        {
            var graph = StoryLoader.LoadFromJson(@"{
                ""title"": ""Test"",
                ""startNodeId"": ""a"",
                ""nodes"": [{ ""id"": ""a"", ""text"": ""Hello world"" }]
            }");

            string json = LocalizationManager.GenerateStarterTable(graph, "fr", "Français");
            StringAssert.Contains("Hello world", json);
        }
    }
}
