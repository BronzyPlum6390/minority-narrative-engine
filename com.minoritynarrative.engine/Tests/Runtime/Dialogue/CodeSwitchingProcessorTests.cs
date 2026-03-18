using NUnit.Framework;
using MinorityNarrativeEngine;

namespace MinorityNarrativeEngine.Tests
{
    public class CodeSwitchingProcessorTests
    {
        private BlackAmericanContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = UnityEngine.ScriptableObject.CreateInstance<BlackAmericanContext>();
            _context.codeSwitchingEnabled = true;
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_context);
        }

        [Test]
        public void Process_YouAll_ReplacedWithYall()
        {
            string result = CodeSwitchingProcessor.Process("you all need to listen", _context);
            StringAssert.Contains("y'all", result);
            StringAssert.DoesNotContain("you all", result);
        }

        [Test]
        public void Process_GoingTo_ReplacedWithFinna()
        {
            string result = CodeSwitchingProcessor.Process("I am going to head out", _context);
            StringAssert.Contains("finna", result);
        }

        [Test]
        public void Process_Disabled_ReturnsOriginalText()
        {
            _context.codeSwitchingEnabled = false;
            string input = "you all need to listen";
            string result = CodeSwitchingProcessor.Process(input, _context);
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Process_NullContext_ReturnsOriginalText()
        {
            string input = "you all need to listen";
            string result = CodeSwitchingProcessor.Process(input, null);
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Process_NullText_ReturnsNull()
        {
            string result = CodeSwitchingProcessor.Process(null, _context);
            Assert.IsNull(result);
        }

        [Test]
        public void Process_EmptyText_ReturnsEmpty()
        {
            string result = CodeSwitchingProcessor.Process("", _context);
            Assert.AreEqual("", result);
        }

        [Test]
        public void Process_WholeWordOnly_DoesNotMatchPartialWord()
        {
            // "family" has wholeWordOnly=true in Latinx context
            var latinx = UnityEngine.ScriptableObject.CreateInstance<LatinxContext>();
            latinx.codeSwitchingEnabled = true;

            // "familiarity" should NOT be replaced
            string result = CodeSwitchingProcessor.Process("I appreciate the familiarity", latinx);
            StringAssert.DoesNotContain("familiaritia", result); // should not corrupt the word
            StringAssert.Contains("familiarity", result);

            UnityEngine.Object.DestroyImmediate(latinx);
        }

        [Test]
        public void Process_CaseInsensitive_MatchesUppercase()
        {
            string result = CodeSwitchingProcessor.Process("You All are welcome here", _context);
            StringAssert.Contains("y'all", result);
        }

        [Test]
        public void Process_MultipleRules_AllApplied()
        {
            string result = CodeSwitchingProcessor.Process("you all going to hear me out for real", _context);
            StringAssert.Contains("y'all", result);
            StringAssert.Contains("finna", result);
            StringAssert.Contains("for real for real", result);
        }
    }
}
