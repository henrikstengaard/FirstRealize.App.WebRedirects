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
        public void ParseUrl()
        {
            var rawUrl = "https://domain.local:8000/path?parameter=value";
            var url = _urlParser.Parse(
                rawUrl);

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "https",
                url.Scheme);
            Assert.AreEqual(
                "domain.local",
                url.Host);
            Assert.AreEqual(
                8000,
                url.Port);
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

        [Test]
        public void ParseUrlStripFragment()
        {
            var rawUrl = "/path#fragment";
            var url = _urlParser.Parse(
                rawUrl,
                stripFragment: true);

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "/path",
                url.PathAndQuery);
            Assert.AreEqual(
                "/path",
                url.Path);
            Assert.AreEqual(
                string.Empty,
                url.Query);
        }

        [Test]
        public void ParseNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _urlParser.Parse(null));
        }

        [Test]
        public void ParseInvalidPortThrowsException()
        {
            Assert.Throws<UriFormatException>(
                () => _urlParser.Parse("http://www.test.local:invalid"));
        }

        [Test]
        public void ParseInvalidUrlThrowsException()
        {
            Assert.Throws<UriFormatException>(
                () => _urlParser.Parse("www.test.local/invalid"));
        }
    }
}