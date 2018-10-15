using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Readers;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReaderTests
{
    [TestFixture]
    public class RedirectCsvReaderTests
    {
        [Test]
        public void ReadRedirects()
        {
            var redirectReader = new RedirectCsvReader(
                Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\redirects.csv"));
            var redirects = redirectReader
                .ReadAllRedirects()
                .ToList();
            Assert.AreNotEqual(0, redirects.Count);

            VerifyRedirect(
                "/example/path",
                "/new-url",
                false,
                false,
                "http://www.test.local/example/path",
                "http://www.test.local/new-url",
                "http://www.test.local/example/path",
                "http://www.test.local/new-url",
                false,
                false,
                redirects[0]);
            VerifyRedirect(
                "/new-url",
                "/another/path",
                false,
                false,
                "http://www.test.local/new-url",
                "http://www.test.local/another/path",
                "http://www.test.local/new-url",
                "http://www.test.local/another/path",
                false,
                false,
                redirects[1]);
            VerifyRedirect(
                "/new-url",
                "/redirect/somwhere/else",
                false,
                false,
                "http://www.test.local/new-url",
                "http://www.test.local/redirect/somwhere/else",
                "http://www.test.local/new-url",
                "http://www.test.local/redirect/somwhere/else",
                false,
                false,
                redirects[2]);
            VerifyRedirect(
                "/another/path",
                "/example/path",
                false,
                false,
                "http://www.test.local/another/path",
                "http://www.test.local/example/path",
                "http://www.test.local/another/path",
                "http://www.test.local/example/path",
                false,
                false,
                redirects[3]);
        }

        [Test]
        public void ReadRedirectsWithWhitespaces()
        {
            var redirectReader = new RedirectCsvReader(
                Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\redirects_whitespace.csv"));
            var redirects = redirectReader
                .ReadAllRedirects()
                .ToList();

            Assert.AreNotEqual(0, redirects.Count);
            Assert.AreEqual(
                "/url1",
                redirects[0].OldUrl);
            Assert.AreEqual(
                "/url2",
                redirects[0].NewUrl);
            Assert.AreEqual(
                "/url1",
                redirects[0].ParsedOldUrl);
            Assert.AreEqual(
                "/url2",
                redirects[0].ParsedNewUrl);
            Assert.AreEqual(
                "/url1",
                redirects[0].OriginalOldUrl);
            Assert.AreEqual(
                "/url2",
                redirects[0].OriginalNewUrl);
        }

        [Test]
        public void ReadRedirectsWithNull()
        {
            var redirectReader = new RedirectCsvReader(
                Path.Combine(TestData.TestData.CurrentDirectory, @"TestData\redirects_null.csv"));
            var redirects = redirectReader
                .ReadAllRedirects()
                .ToList();

            Assert.AreNotEqual(0, redirects.Count);
            Assert.AreEqual(
                "/url1",
                redirects[0].OldUrl);
            Assert.AreEqual(
                "/url2",
                redirects[0].NewUrl);
        }

        private void VerifyRedirect(
            string oldUrl,
            string newUrl,
            bool oldUrlHasHost,
            bool newUrlHasHost,
            string parsedOldUrl,
            string parsedNewUrl,
            string originalOldUrl,
            string originalNewUrl,
            bool originalOldUrlHasHost,
            bool originalNewUrlHasHost,
            IRedirect redirect)
        {
            Assert.AreEqual(
                oldUrl,
                redirect.OldUrl);
            Assert.AreEqual(
                newUrl,
                redirect.NewUrl);
            Assert.AreEqual(
                oldUrlHasHost,
                redirect.OldUrlHasHost);
            Assert.AreEqual(
                newUrlHasHost,
                redirect.NewUrlHasHost);
            Assert.AreEqual(
                parsedOldUrl,
                redirect.ParsedOldUrl);
            Assert.AreEqual(
                parsedNewUrl,
                redirect.ParsedNewUrl);
            Assert.AreEqual(
                originalOldUrl,
                redirect.OriginalOldUrl);
            Assert.AreEqual(
                originalNewUrl,
                redirect.OriginalNewUrl);
            Assert.AreEqual(
                originalOldUrlHasHost,
                redirect.OriginalOldUrlHasHost);
            Assert.AreEqual(
                originalNewUrlHasHost,
                redirect.OriginalNewUrlHasHost);
        }
    }
}