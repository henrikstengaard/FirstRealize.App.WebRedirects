using FirstRealize.App.WebRedirects.Core.Configuration;
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

            var redirect = redirectParser.ParseRedirect(
                "/old-url/#anchor",
                "/new-url/#anchor");

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