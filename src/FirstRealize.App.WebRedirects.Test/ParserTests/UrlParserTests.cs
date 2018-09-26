using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System;

namespace FirstRealize.App.WebRedirects.Test.ParserTests
{
    [TestFixture]
    public class UrlParserTests
    {
        private readonly IUrlParser _urlParser;

        public UrlParserTests()
        {
            _urlParser = new UrlParser(
                TestData.TestData.DefaultConfiguration);
        }

        [Test]
        public void ParseUrlWithAbsoluteUri()
        {
            var rawUrl = "http://www.example.local";
            var url = _urlParser.ParseUrl(
                rawUrl);

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "http",
                url.Parsed.Scheme);
            Assert.AreEqual(
                "www.example.local",
                url.Parsed.DnsSafeHost);
        }

        [Test]
        public void ParseUrlWithOnlyPathUsingHost()
        {
            var rawUrl = "/a/path";
            var url = _urlParser.ParseUrl(
                rawUrl,
                new Uri("http://www.defaulthost.local"));

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "http",
                url.Parsed.Scheme);
            Assert.AreEqual(
                "www.defaulthost.local",
                url.Parsed.DnsSafeHost);
            Assert.AreEqual(
                "/a/path",
                url.Parsed.AbsolutePath);
        }

        [Test]
        public void ParseUrlReturnsNullForUrlWithoutSchemeOrSlash()
        {
            var rawUrl = "example.local/another/path";
            var url = _urlParser.ParseUrl(
                rawUrl);

            Assert.IsNotNull(url);
            Assert.IsNull(url.Parsed);
            Assert.AreEqual(
                "example.local/another/path",
                url.Raw);
        }

        [Test]
        public void ParseUrlRemovesTailingSlash()
        {
            var rawUrl = "/another/path/";
            var url = _urlParser.ParseUrl(
                rawUrl,
                TestData.TestData.DefaultHost);

            Assert.AreEqual(
                "http://www.test.local/another/path",
                url.Parsed.AbsoluteUri);
        }

        [Test]
        public void ParseUrlRemovesFragment()
        {
            var rawUrl = "/another/path/#anchor";
            var url = _urlParser.ParseUrl(
                rawUrl,
                TestData.TestData.DefaultHost,
                true);

            Assert.AreEqual(
                "http://www.test.local/another/path",
                url.Parsed.AbsoluteUri);
        }

        [Test]
        public void ParseUrlWithOnlyPathDoesntHaveHost()
        {
            var rawUrl = "/another/path/";
            var url = _urlParser.ParseUrl(
                rawUrl,
                TestData.TestData.DefaultHost);

            Assert.AreEqual(
                false,
                url.HasHost);
        }

        [Test]
        public void ParseUrlWithAbsoluteUriHasHost()
        {
            var rawUrl = "http://example.local/another/path/";
            var url = _urlParser.ParseUrl(
                rawUrl);

            Assert.AreEqual(
                true,
                url.HasHost);
        }

        [Test]
        public void ParseUrlWithQueryString()
        {
            var rawUrl = "/path?parameter=value";
            var url = _urlParser.Parse(
                rawUrl);

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "/path?parameter=value",
                url.PathAndQuery);
            Assert.AreEqual(
                "/path",
                url.Path);
            Assert.AreEqual(
                "parameter=value",
                url.Query);
        }
    }
}