using FirstRealize.App.WebRedirects.Core.Models.Urls;
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
            _urlParser = new UrlParser();
        }

        [Test]
        public void ParseUrlWithAbsoluteUri()
        {
            var rawUrl = "http://www.example.local";
            var url = _urlParser.Parse(rawUrl);

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "http",
                url.Scheme);
            Assert.AreEqual(
                "www.example.local",
                url.Host);
        }

        [Test]
        public void ParseUrlWithOnlyPathUsingHost()
        {
            var rawUrl = "/a/path";
            var url = _urlParser.Parse(rawUrl,
                new ParsedUrl
                {
                    Scheme = "http",
                    Host = "www.defaulthost.local",
                    Port = 80
                });

            Assert.IsNotNull(url);
            Assert.AreEqual(
                "http",
                url.Scheme);
            Assert.AreEqual(
                "www.defaulthost.local",
                url.Host);
            Assert.AreEqual(
                "/a/path",
                url.Path);
        }

        [Test]
        public void ParseUrlWithOnlyPathDoesntHaveHost()
        {
            var rawUrl = "/another/path/";
            var url = _urlParser.Parse(rawUrl,
                TestData.TestData.DefaultHost);

            Assert.AreEqual(
                false,
                url.OriginalUrlHasHost);
        }

        [Test]
        public void ParseUrlWithAbsoluteUriHasHost()
        {
            var rawUrl = "http://example.local/another/path/";
            var url = _urlParser.Parse(rawUrl);

            Assert.AreEqual(
                true,
                url.OriginalUrlHasHost);
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
            Assert.AreEqual(
                rawUrl,
                url.OriginalUrl);
            Assert.AreEqual(
                true,
                url.OriginalUrlHasHost);
        }

        [Test]
        public void ParseUrlWithQueryString()
        {
            var rawUrl = "/path?parameter=value";
            var url = _urlParser.Parse(
                rawUrl,
                TestData.TestData.DefaultHost);

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
            Assert.AreEqual(
                rawUrl,
                url.OriginalUrl);
            Assert.AreEqual(
                false,
                url.OriginalUrlHasHost);
        }

        [Test]
        public void ParseUrlStripFragment()
        {
            var rawUrl = "/path#fragment";
            var url = _urlParser.Parse(
                rawUrl,
                TestData.TestData.DefaultHost,
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
            Assert.AreEqual(
                rawUrl,
                url.OriginalUrl);
            Assert.AreEqual(
                false,
                url.OriginalUrlHasHost);
        }

        [Test]
        public void ParseNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _urlParser.Parse(null));
        }

        [Test]
        public void ParseInvalidPort()
        {
            var parsedUrl = _urlParser.Parse("http://www.test.local:invalid");

            Assert.IsNotNull(parsedUrl);
            Assert.AreEqual(
                false,
                parsedUrl.IsValid);
        }

        [Test]
        public void ParseInvalidUrl()
        {
            var parsedUrl = _urlParser.Parse("www.test.local/invalid");

            Assert.IsNotNull(parsedUrl);
            Assert.AreEqual(
                false,
                parsedUrl.IsValid);
        }
    }
}