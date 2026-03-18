using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class HonorificsResolverTests
    {
        private BlackAmericanContext _context;
        private StorySession _session;

        [SetUp]
        public void SetUp()
        {
            _context = UnityEngine.ScriptableObject.CreateInstance<BlackAmericanContext>();
            _session = new StorySession();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_context);
        }

        [Test]
        public void Resolve_ElderToken_ReplacedWithContextForm()
        {
            string result = HonorificsResolver.Resolve(
                "Listen to me, {honorific.elder}.", _context, _session, "isaiah", "ma_ellen");
            StringAssert.DoesNotContain("{honorific.elder}", result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void Resolve_NoTokens_ReturnsOriginalText()
        {
            string input = "No honorifics here.";
            string result = HonorificsResolver.Resolve(input, _context, _session, "isaiah");
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Resolve_NullContext_ReturnsOriginalText()
        {
            string input = "Listen, {honorific.elder}.";
            string result = HonorificsResolver.Resolve(input, null, _session, "isaiah");
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Resolve_NullText_ReturnsNull()
        {
            string result = HonorificsResolver.Resolve(null, _context, _session, "isaiah");
            Assert.IsNull(result);
        }

        [Test]
        public void Resolve_MultipleTokens_AllReplaced()
        {
            string result = HonorificsResolver.Resolve(
                "{honorific.elder} and {honorific.family} are here.",
                _context, _session, "isaiah");

            StringAssert.DoesNotContain("{honorific.elder}", result);
            StringAssert.DoesNotContain("{honorific.family}", result);
        }

        [Test]
        public void Resolve_IndigenousContext_UsesCorrectForm()
        {
            var indigenous = UnityEngine.ScriptableObject.CreateInstance<IndigenousContext>();

            string result = HonorificsResolver.Resolve(
                "Thank you, {honorific.elder}.", indigenous, _session, "player");

            StringAssert.Contains("Elder", result);
            UnityEngine.Object.DestroyImmediate(indigenous);
        }
    }
}
