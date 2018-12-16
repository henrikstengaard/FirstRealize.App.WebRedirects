using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ExportTests
{
    [TestFixture]
    public class WebConfigExporterTests
    {
        [Test]
        public void BuildWebConfig()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var webConfigExporter = new WebConfigExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.domain1.local/url1",
                    NewUrl = "http://www.domain2.local/url2",
                    OldUrlHasHost = true,
                    NewUrlHasHost = true,
                    ParsedOldUrl = "http://www.domain1.local/url1",
                    ParsedNewUrl = "http://www.domain2.local/url2",
                    OriginalOldUrl = "http://www.domain1.local/url1",
                    OriginalNewUrl = "http://www.domain2.local/url2",
                },
                new Redirect
                {
                    OldUrl = "http://www.test.local/url1",
                    NewUrl = "http://www.test.local/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url1",
                    ParsedNewUrl = "http://www.test.local/url2",
                    OriginalOldUrl = "/url1",
                    OriginalNewUrl = "/url2",
                },
                new Redirect
                {
                    OldUrl = "http://www.test.local/url2",
                    NewUrl = "http://www.test.local/url3",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url2",
                    ParsedNewUrl = "http://www.test.local/url3",
                    OriginalOldUrl = "/url2",
                    OriginalNewUrl = "/url3",
                }
            };

            var webConfig = webConfigExporter.Build(
                redirects);

            // verify web config rewrite rules
            Assert.IsNotNull(webConfig);
            var webConfigLines = webConfig
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                webConfigLines.Count);
            Assert.AreEqual(
                2,
                webConfigLines.Count(x => x.Contains("<rule name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^url1/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"http://www.domain2.local/url2\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add input=\"{HTTP_HOST}\" pattern=\"^www.domain1.local$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^(.+?)/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"{C:1}\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));

            // verify web config rewrite maps
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rewriteMap name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"url2\" value=\"/url3\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"url1\" value=\"/url2\" />")));
        }

        [Test]
        public void BuildWebConfigReplaceRedirectTypeWith1Segment()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var webConfigExporter = new WebConfigExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.test.local/url1",
                    NewUrl = "http://www.test.local/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url1",
                    ParsedNewUrl = "http://www.test.local/url2",
                    OriginalOldUrl = "http://www.test.local/url1",
                    OriginalNewUrl = "http://www.test.local/url2",
                    RedirectType = RedirectType.Replace
                }
            };

            var webConfig = webConfigExporter.Build(
                redirects);

            // verify web config rewrite rules
            Assert.IsNotNull(webConfig);
            var webConfigLines = webConfig
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                webConfigLines.Count);
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rule name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^url1(.+)?/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"/url2{R:1}\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<add input=\"{HTTP_HOST}\" pattern=\"^www.test.local$\" />")));

            // verify web config doesn't have rewrite maps
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<rewriteMap name=")));
        }

        [Test]
        public void BuildWebConfigReplaceRedirectTypeWith1SegmentAndRewriteMaps()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var webConfigExporter = new WebConfigExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.test.local/url1",
                    NewUrl = "http://www.test.local/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url1",
                    ParsedNewUrl = "http://www.test.local/url2",
                    OriginalOldUrl = "http://www.test.local/url1",
                    OriginalNewUrl = "http://www.test.local/url2",
                    RedirectType = RedirectType.Replace
                },
                new Redirect
                {
                    OldUrl = "http://www.test.local/url2",
                    NewUrl = "http://www.test.local/url3",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/url2",
                    ParsedNewUrl = "http://www.test.local/url3",
                    OriginalOldUrl = "/url2",
                    OriginalNewUrl = "/url3",
                    RedirectType = RedirectType.Replace
                }
            };

            var webConfig = webConfigExporter.Build(
                redirects);

            // verify web config rewrite rules
            Assert.IsNotNull(webConfig);
            var webConfigLines = webConfig
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                webConfigLines.Count);
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rule name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^([^/]+)(.+)?/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"{C:1}{R:2}\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<add input=\"{HTTP_HOST}\" pattern=\"^www.test.local$\" />")));

            // verify web config rewrite maps
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rewriteMap name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"url2\" value=\"/url3\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"url1\" value=\"/url2\" />")));
        }

        [Test]
        public void BuildWebConfigReplaceRedirectTypeWith2Segments()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var webConfigExporter = new WebConfigExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.test.local/old1/old2",
                    NewUrl = "http://www.test.local/new1/new2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/old1/old2",
                    ParsedNewUrl = "http://www.test.local/new1/new2",
                    OriginalOldUrl = "http://www.test.local/old1/old2",
                    OriginalNewUrl = "http://www.test.local/new1/new2",
                    RedirectType = RedirectType.Replace
                }
            };

            var webConfig = webConfigExporter.Build(
                redirects);

            // verify web config rewrite rules
            Assert.IsNotNull(webConfig);
            var webConfigLines = webConfig
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                webConfigLines.Count);
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rule name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^old1/old2(.+)?/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"/new1/new2{R:1}\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<add input=\"{HTTP_HOST}\" pattern=\"^www.test.local$\" />")));

            // verify web config doesn't have rewrite maps
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<rewriteMap name=")));
        }

        [Test]
        public void BuildWebConfigReplaceRedirectTypeWith2SegmentsAndRewriteMaps()
        {
            var configuration = TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var webConfigExporter = new WebConfigExporter(
                configuration,
                urlParser,
                new UrlFormatter());

            var redirects = new[]
            {
                new Redirect
                {
                    OldUrl = "http://www.test.local/old1/old2",
                    NewUrl = "http://www.test.local/new1/new2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/old1/old2",
                    ParsedNewUrl = "http://www.test.local/new1/new2",
                    OriginalOldUrl = "http://www.test.local/old1/old2",
                    OriginalNewUrl = "http://www.test.local/new1/new2",
                    RedirectType = RedirectType.Replace
                },
                new Redirect
                {
                    OldUrl = "http://www.test.local/one1/one2",
                    NewUrl = "http://www.test.local/two1/two2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    ParsedOldUrl = "http://www.test.local/one1/one2",
                    ParsedNewUrl = "http://www.test.local/two1/two2",
                    OriginalOldUrl = "/one1/one2",
                    OriginalNewUrl = "/two1/two2",
                    RedirectType = RedirectType.Replace
                }
            };

            var webConfig = webConfigExporter.Build(
                redirects);

            // verify web config rewrite rules
            Assert.IsNotNull(webConfig);
            var webConfigLines = webConfig
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            Assert.AreNotEqual(
                0,
                webConfigLines.Count);
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rule name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<match url=\"^([^/]+/[^/]+)(.+)?/?$\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<action type=\"Redirect\" url=\"{C:1}{R:2}\" redirectType=\"Permanent\" appendQueryString=\"False\" />")));
            Assert.AreEqual(
                0,
                webConfigLines.Count(x => x.Contains("<add input=\"{HTTP_HOST}\" pattern=\"^www.test.local$\" />")));

            // verify web config rewrite maps
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<rewriteMap name=")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"old1/old2\" value=\"/new1/new2\" />")));
            Assert.AreEqual(
                1,
                webConfigLines.Count(x => x.Contains("<add key=\"one1/one2\" value=\"/two1/two2\" />")));
        }
    }
}