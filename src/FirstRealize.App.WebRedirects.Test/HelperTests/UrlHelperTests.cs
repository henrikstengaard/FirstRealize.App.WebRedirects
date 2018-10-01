using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Test.HelperTests
{
    [TestFixture]
    public class UrlHelperTests
    {
        private readonly IUrlHelper _urlHelper;

        public UrlHelperTests()
        {
            // create url helper
			var configuration =
				TestData.TestData.DefaultConfiguration;

			var urlParser = new UrlParser();
			var urlFormatter = new UrlFormatter();
			_urlHelper = new UrlHelper(
                configuration,
				urlParser,
				urlFormatter);
        }

        [Test]
        public void DetectHttpsRedirect()
        {
            // create urls
            var url1 = "http://www.test.local/url1";
            var url2 = "https://www.test.local/url1";

            // verify urls is https redirect
            Assert.AreEqual(
                true,
                _urlHelper.IsHttpsRedirect(
					url1,
					url2));
        }

        [Test]
        public void HttpUrlsIsNotHttpsRedirect()
        {
            // create urls
            var url1 = "http://www.test.local/url1";
            var url2 = "http://www.test.local/url1";

            // verify urls is not https redirect with only http scheme
            Assert.AreEqual(
                false,
                _urlHelper.IsHttpsRedirect(
					url1,
					url2));
        }

        [Test]
        public void UrlsAreIdentical()
        {
            // create urls
            var url1 = "http://www.test.local/url1";
            var url2 = "http://www.test.local/url1";

            // verify urls are identical
            Assert.AreEqual(
                true,
                _urlHelper.AreIdentical(
					url1,
					url2));
        }

        [Test]
        public void UrlsWithDifferentSchemeAreNotIdentical()
        {
            // create configuration without force http host patterns
            var configuration = TestData.TestData.DefaultConfiguration;
            configuration.ForceHttpHostPatterns = new List<string>();
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser();
			var urlHelper = new UrlHelper(
				configuration,
				urlParser,
				urlFormatter);

			// create urls
			var rawUrl1 = "http://www.test.local/url1";
            var rawUrl2 = "https://www.test.local/url1";
            var parsedUrl1 = urlParser.Parse(
                rawUrl1,
                configuration.DefaultUrl);
            var parsedUrl2 = urlParser.Parse(
                rawUrl2,
                configuration.DefaultUrl);
            var url1 = new Url
            {
                Raw = rawUrl1,
                Parsed = parsedUrl1,
                Formatted = urlFormatter.Format(
                    parsedUrl1)
            };
            var url2 = new Url
            {
                Raw = rawUrl2,
                Parsed = parsedUrl2,
                Formatted = urlFormatter.Format(
                    parsedUrl2)
            };

            // verify urls are identical using force http host pattern and one url has https scheme
            Assert.AreEqual(
                false,
                urlHelper.AreIdentical(
					url1.Formatted,
					url2.Formatted));
        }

		[Test]
		public void CombineUrls()
		{
			var urlCombined = _urlHelper.Combine(
				"http://www.url1.local/some/path",
				"http://www.url2.local/path/to/combine");

			Assert.IsNotNull(urlCombined);
			Assert.AreEqual(
				"http://www.url1.local/path/to/combine",
				urlCombined);
		}

		[Test]
		public void CombineUrlsWhereSecondUrlStartsWithSlash()
		{
			var urlCombined = _urlHelper.Combine(
				"http://www.url1.local/some/path",
				"/path/to/combine");

			Assert.IsNotNull(urlCombined);
			Assert.AreEqual(
				"http://www.url1.local/path/to/combine",
				urlCombined);
		}

		[Test]
		public void CombineUrlsWhereBothUrlsStartsWithSlash()
		{
			var urlCombined = _urlHelper.Combine(
				"/some/path",
				"/path/to/combine");

			Assert.IsNotNull(urlCombined);
			Assert.AreEqual(
				"http://www.test.local/path/to/combine",
				urlCombined);
		}
	}
}