using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ModelTests
{
    [TestFixture]
    public class RedirectTests
    {
        [Test]
        public void SortRedirectsInAscendingOrder()
        {
            var redirects = TestData.TestData
                .GetParsedRedirects()
                .ToList();

            redirects.Sort();

            Assert.AreEqual(4, redirects.Count);

            Assert.AreEqual(
                "http://www.test.local/another/path",
                redirects[0].OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/example/path",
                redirects[0].NewUrl.Parsed.AbsoluteUri);

            Assert.AreEqual(
                "http://www.test.local/example/path",
                redirects[1].OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                redirects[1].NewUrl.Parsed.AbsoluteUri);

            Assert.AreEqual(
                "http://www.test.local/new-url",
                redirects[2].OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/another/path",
                redirects[2].NewUrl.Parsed.AbsoluteUri);

            Assert.AreEqual(
                "http://www.test.local/new-url",
                redirects[3].OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                redirects[3].NewUrl.Parsed.AbsoluteUri);
        }
    }
}