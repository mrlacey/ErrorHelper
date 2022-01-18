using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErrorHelper.Tests
{
    [TestClass]
    public class StripPaths
    {
        [TestMethod]
        public void NoPaths()
        {
            InputProducesExpected("abcdefghijkl", "abcdefghijkl");
        }

        [TestMethod]
        public void Null()
        {
            InputProducesExpected(null, string.Empty);
        }

        [TestMethod]
        public void EmptyString()
        {
            InputProducesExpected(string.Empty, string.Empty);
        }

        [TestMethod]
        public void WhiteSpace()
        {
            InputProducesExpected("     ", "     ");
        }

        [TestMethod]
        public void SpacesAround_AbsolutePath()
        {
            InputProducesExpected(
                "something is wrong in C:/path/file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_AbsolutePath()
        {
            InputProducesExpected(
                "something is wrong in 'C:/path/file.ext' that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SpacesAround_RelativePath()
        {
            InputProducesExpected(
                "something is wrong in ../otherpath/file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_RelativePath()
        {
            InputProducesExpected(
                "something is wrong in '../otherpath/file.ext' that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SpacesAround_Uri_NotRemoved()
        {
            InputProducesExpected(
                "something is wrong in http://example.com that needs fixing.",
                "something is wrong in http://example.com that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_Uri_NotRemoved()
        {
            InputProducesExpected(
                "something is wrong in 'http://example.com' that needs fixing.",
                "something is wrong in 'http://example.com' that needs fixing.");
        }

        private void InputProducesExpected(string input, string expected)
        {
            var actual = SearchDescriptionWithoutPathsCommand.StripPaths(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
