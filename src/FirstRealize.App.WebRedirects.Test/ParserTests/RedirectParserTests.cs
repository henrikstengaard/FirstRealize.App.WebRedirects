using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;

namespace FirstRealize.App.WebRedirects.Test.ParserTests
{
    [TestFixture]
    public class RedirectParserTests
    {
        [Test]
        public void CanParseRedirect()
        {
            var redirectParser = new RedirectParser(
                TestData.TestData.DefaultConfiguration,
                new UrlParser());

            var redirect = new Redirect
            {
                OldUrl = new Url
                {
                    Raw = "/old-url/#anchor"
                },
                NewUrl = new Url
                {
                    Raw = "/new-url/#anchor"
                }
            };

            redirectParser.ParseRedirect(
                redirect);

            Assert.IsNotNull(redirect);
            Assert.AreEqual(
                "http://www.test.local/old-url",
                redirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url#anchor",
                redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}