using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System;

namespace FirstRealize.App.WebRedirects.Test.ParserTests
{
    [TestFixture]
    public class UrlParserTests
    {
        private readonly IUrlParser _urlParser;
        private readonly RedirectProcessor _redirectProcessor;

        public UrlParserTests()
        {
            _urlParser = new UrlParser();
            _redirectProcessor = new RedirectProcessor(
                new Configuration());
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
        public void CanRemoveTailingSlash()
        {
            var url = "/another/path/";
            var uri = _urlParser.ParseUrl(
                url,
                TestData.TestData.DefaultHost);

            Assert.AreEqual(
                "http://www.test.local/another/path", 
                uri.AbsoluteUri);
        }

        [Test]
        public void CanRemoveFragment()
        {
            var url = "/another/path/#anchor";
            var uri = _urlParser.ParseUrl(
                url,
                TestData.TestData.DefaultHost,
                true);

            Assert.AreEqual(
                "http://www.test.local/another/path",
                uri.AbsoluteUri);
        }
    }
}