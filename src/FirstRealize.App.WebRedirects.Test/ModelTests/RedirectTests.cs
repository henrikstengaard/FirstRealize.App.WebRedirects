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
            var parsedRedirects = TestData.TestData
                .GetParsedRedirects()
                .ToList();

            parsedRedirects.Sort();

            Assert.AreEqual(4, parsedRedirects.Count);

            Assert.AreEqual(
                "http://www.test.local/another/path",
                parsedRedirects[0].OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/example/path",
                parsedRedirects[0].NewUrl.Formatted);

            Assert.AreEqual(
                "http://www.test.local/example/path",
                parsedRedirects[1].OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                parsedRedirects[1].NewUrl.Formatted);

            Assert.AreEqual(
                "http://www.test.local/new-url",
                parsedRedirects[2].OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/another/path",
                parsedRedirects[2].NewUrl.Formatted);

            Assert.AreEqual(
                "http://www.test.local/new-url",
                parsedRedirects[3].OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                parsedRedirects[3].NewUrl.Formatted);
        }
    }
}