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
        public void SpacesAround_AbsolutePath_ForwardSlash()
        {
            InputProducesExpected(
                "something is wrong in C:/path/file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_AbsolutePath_ForwardSlash()
        {
            InputProducesExpected(
                "something is wrong in 'C:/path/file.ext' that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SpacesAround_RelativePath_ForwardSlash()
        {
            InputProducesExpected(
                "something is wrong in ../otherpath/file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_RelativePath_ForwardSlash()
        {
            InputProducesExpected(
                "something is wrong in '../otherpath/file.ext' that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SpacesAround_AbsolutePath_BackwardSlash()
        {
            InputProducesExpected(
                "something is wrong in C:\\path\\file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_AbsolutePath_BackwardSlash()
        {
            InputProducesExpected(
                "something is wrong in 'C:\\path\\file.ext' that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SpacesAround_RelativePath_BackwardSlash()
        {
            InputProducesExpected(
                "something is wrong in ..\\otherpath\\file.ext that needs fixing.",
                "something is wrong in  that needs fixing.");
        }

        [TestMethod]
        public void SingleQuoteAround_RelativePath_BackwardSlash()
        {
            InputProducesExpected(
                "something is wrong in '..\\otherpath\\file.ext' that needs fixing.",
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

        [TestMethod]
        public void QuotedUrlAtEndOfString_NoError()
        {
            InputProducesExpected(
                "something is wrong in \"http://example.com\"",
                "something is wrong in \"http://example.com\"");
        }

        [TestMethod]
        public void Issue13()
        {
            InputProducesExpected(
                "\"double Triangle::getArea(int x) const\" (declared at line 11 of \"C:\\Users\\Lars\\OneDrive\\programming\\geomalgorithms\\geomalgorithms\\triangle.h\")",
                "\"double Triangle::getArea(int x) const\" (declared at line 11 of \"\")");
        }

        private void InputProducesExpected(string input, string expected)
        {
            var actual = SearchDescriptionWithoutPathsCommand.StripPaths(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
