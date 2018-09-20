using FirstRealize.App.WebRedirects.Core.Models.Redirects;
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
                OldUrl = "/old-url/#anchor",
                NewUrl = "/new-url/#anchor"
            };

            var parsedRedirect = 
                redirectParser.ParseRedirect(
                    redirect);

            Assert.IsNotNull(redirect);
            Assert.AreEqual(
                "http://www.test.local/old-url",
                parsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url#anchor",
                parsedRedirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}