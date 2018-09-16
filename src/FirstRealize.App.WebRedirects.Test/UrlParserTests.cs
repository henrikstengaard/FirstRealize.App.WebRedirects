using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test
{
    [TestFixture]
    public class UrlParserTests
    {
        private readonly UrlParser _urlParser;
        private readonly RedirectProcessor _redirectProcessor;

        public UrlParserTests()
        {
            _urlParser = new UrlParser();
            _redirectProcessor = new RedirectProcessor();
        }

        [Test]
        public void ParseUrlWithHttp()
        {
            var url = "http://www.example.local";
            var uri = _urlParser.ParseUrl(url);

            Assert.IsNotNull(uri);
            Assert.AreEqual("http", uri.Scheme);
            Assert.AreEqual("www.example.local", uri.DnsSafeHost);
        }

        [Test]
        public void ParseUrlPathWithDefaultHost()
        {
            var url = "/a/path";
            var uri = _urlParser.ParseUrl(
                url, 
                new Uri("http://www.defaulthost.local"));

            Assert.IsNotNull(uri);
            Assert.AreEqual("http", uri.Scheme);
            Assert.AreEqual("www.defaulthost.local", uri.DnsSafeHost);
            Assert.AreEqual("/a/path", uri.AbsolutePath);
        }

        [Test]
        public void ParseUrlWithDomainAndPath()
        {
            var url = "example.local/another/path";
            var uri = _urlParser.ParseUrl(
                url);

            Assert.IsNotNull(uri);
            Assert.AreEqual("http", uri.Scheme);
            Assert.AreEqual("example.local", uri.DnsSafeHost);
            Assert.AreEqual("/another/path", uri.AbsolutePath);
        }

        [Test]
        public void ParseUrlPathWithoutDefaultHost()
        {
            var url = "another/path";
            var uri = _urlParser.ParseUrl(
                url);

            Assert.IsNull(uri);
        }

        [Test]
        public void ParseUrlPoc()
        {
            // 1. raw urls
            // 2. parse urls
            // 3. process redirects
            // 4. check redirects
            // 5. format redirects
            // 6. write redirects

            var processedRedirects =
                new List<ProcessedRedirect>();

            foreach (var redirect in TestData.GetRedirects())
            {
                // parse urls
                redirect.OldUrl.Parsed = _urlParser.ParseUrl(
                    redirect.OldUrl.Raw,
                    TestData.DefaultHost);
                redirect.NewUrl.Parsed = _urlParser.ParseUrl(
                    redirect.NewUrl.Raw,
                    TestData.DefaultHost);

                // process redirect
                var processedRedirect = 
                    _redirectProcessor.Process(redirect);

                //Assert.IsTrue(redirect.OldUrl.IsValid);
                Assert.IsTrue(redirect.NewUrl.IsValid);

                processedRedirects.Add(
                    processedRedirect);
            }

            var duplicate = processedRedirects
                .FirstOrDefault(x => x.IsDuplicate);
            Assert.IsNotNull(duplicate);
            Assert.AreEqual(
                "http://www.test.local/another/path",
                duplicate.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}