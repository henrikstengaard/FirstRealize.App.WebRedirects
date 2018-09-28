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
            var urlParser = new UrlParser(
                configuration);
            var webConfigExporter = new WebConfigExporter(
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
                    OldUrlParsed = "http://www.domain1.local/url1",
                    NewUrlParsed = "http://www.domain2.local/url2",
                    OldUrlRefined = "http://www.domain1.local/url1",
                    NewUrlRefined = "http://www.domain2.local/url2",
                },
                new Redirect
                {
                    OldUrl = "/url1",
                    NewUrl = "/url2",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    OldUrlParsed = "http://www.test.local/url1",
                    NewUrlParsed = "http://www.test.local/url2",
                    OldUrlRefined = "http://www.test.local/url1",
                    NewUrlRefined = "http://www.test.local/url2",
                },
                new Redirect
                {
                    OldUrl = "/url2",
                    NewUrl = "/url3",
                    OldUrlHasHost = false,
                    NewUrlHasHost = false,
                    OldUrlParsed = "http://www.test.local/url2",
                    NewUrlParsed = "http://www.test.local/url3",
                    OldUrlRefined = "http://www.test.local/url2",
                    NewUrlRefined = "http://www.test.local/url3",
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
    }
}