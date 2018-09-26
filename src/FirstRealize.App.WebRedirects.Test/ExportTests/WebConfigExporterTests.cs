using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;

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

            Assert.IsNotNull(webConfig);
        }
    }
}