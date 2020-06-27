using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErrorHelper.Tests
{
    [TestClass]
    public class ParseUrls
    {
        [TestMethod]
        public void EmptyString()
        {
            InputProducesExpected(string.Empty, new List<string>());
        }

        [TestMethod]
        public void NoUrls()
        {
            InputProducesExpected("just some regular text without any urls", new List<string>());
        }

        [TestMethod]
        public void JustOneUrl()
        {
            InputProducesExpected(
                "https://rapidxaml.dev",
                new List<string> { "https://rapidxaml.dev" });
        }

        [TestMethod]
        public void OneUrl_AtStart()
        {
            InputProducesExpected(
                "https://rapidxaml.dev followed by some text",
                new List<string> { "https://rapidxaml.dev" });
        }

        [TestMethod]
        public void OneUrl_InMiddle()
        {
            InputProducesExpected(
                "Some text with a single url https://rapidxaml.dev followed by some more text",
                new List<string> { "https://rapidxaml.dev" });
        }

        [TestMethod]
        public void OneUrl_AtEnd()
        {
            InputProducesExpected(
                "Some text with a single url at the end https://rapidxaml.dev",
                new List<string> { "https://rapidxaml.dev" });
        }

        [TestMethod]
        public void ManyUrls_OneLine()
        {
            InputProducesExpected(
                "Some text with two urls. The first is https://rapidxaml.dev and the second is https://mrlacey.com",
                new List<string> { "https://rapidxaml.dev", "https://mrlacey.com" });
        }

        [TestMethod]
        public void ManyUrls_MultipleLines()
        {
            InputProducesExpected(
                @"Some text with two urls.
The first is https://rapidxaml.dev and
the second is https://mrlacey.com/book",
                new List<string> { "https://rapidxaml.dev", "https://mrlacey.com/book" });
        }

        [TestMethod]
        public void RealWorldExample()
        {
            InputProducesExpected(
                "The certificate specified has expired. For more information about renewing certificates, see http://go.microsoft.com/fwlink/?LinkID=241478.",
                new List<string> { "http://go.microsoft.com/fwlink/?LinkID=241478" });
        }

        private void InputProducesExpected(string input, List<string> expected)
        {
            var actual = OpenUrlCommand.ParseUrls(input);

            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
