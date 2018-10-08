using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ModelTests
{
    [TestFixture]
    public class RedirectTests
    {
        [Test]
        public void SortInvalidParsedRedirects()
        {
            var parsedRedirects = TestData.TestData.GetParsedRedirects(
                new[]
                {
                    new Redirect
                    {
                        OldUrl = string.Empty,
                        NewUrl = string.Empty
                    },
                    new Redirect
                    {
                        OldUrl = string.Empty,
                        NewUrl = "/url1"
                    },
                    new Redirect
                    {
                        OldUrl = "/url1",
                        NewUrl = "/url2"
                    }
                })
                .ToList();

            parsedRedirects.Sort();

            Assert.AreEqual(3, parsedRedirects.Count);
            Assert.AreEqual(
                false,
                parsedRedirects[0].IsValid);
            Assert.AreEqual(
                string.Empty,
                parsedRedirects[0].OldUrl.Raw);
            Assert.AreEqual(
                "/url1",
                parsedRedirects[0].NewUrl.Raw);
            Assert.AreEqual(
                false,
                parsedRedirects[1].IsValid);
            Assert.AreEqual(
                string.Empty,
                parsedRedirects[1].OldUrl.Raw);
            Assert.AreEqual(
                string.Empty,
                parsedRedirects[1].NewUrl.Raw);
            Assert.AreEqual(
                true,
                parsedRedirects[2].IsValid);
            Assert.AreEqual(
                "http://www.test.local/url1",
                parsedRedirects[2].OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/url2",
                parsedRedirects[2].NewUrl.Formatted);
        }

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