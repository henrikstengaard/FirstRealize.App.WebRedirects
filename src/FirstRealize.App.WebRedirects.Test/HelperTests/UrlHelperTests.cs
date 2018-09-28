using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Parsers;
using NUnit.Framework;
using System;
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

			var urlParser = new UrlParser(
				configuration);
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
            var rawUrl1 = "http://www.test.local/url1";
            var rawUrl2 = "https://www.test.local/url1";
            var url1 = new Url
            {
                Raw = rawUrl1,
                Parsed = new Uri(rawUrl1)
            };
            var url2 = new Url
            {
                Raw = rawUrl2,
                Parsed = new Uri(rawUrl2)
            };

            // verify urls is https redirect
            Assert.AreEqual(
                true,
                _urlHelper.IsHttpsRedirect(
					url1.Parsed.AbsoluteUri,
					url2.Parsed.AbsoluteUri));
        }

        [Test]
        public void HttpUrlsIsNotHttpsRedirect()
        {
            // create urls
            var rawUrl1 = "http://www.test.local/url1";
            var rawUrl2 = "http://www.test.local/url1";
            var url1 = new Url
            {
                Raw = rawUrl1,
                Parsed = new Uri(rawUrl1)
            };
            var url2 = new Url
            {
                Raw = rawUrl2,
                Parsed = new Uri(rawUrl2)
            };

            // verify urls is not https redirect with only http scheme
            Assert.AreEqual(
                false,
                _urlHelper.IsHttpsRedirect(
					url1.Parsed.AbsoluteUri,
					url2.Parsed.AbsoluteUri));
        }

        [Test]
        public void UrlsAreIdentical()
        {
            // create urls
            var rawUrl1 = "http://www.test.local/url1";
            var rawUrl2 = "http://www.test.local/url1";
            var url1 = new Url
            {
                Raw = rawUrl1,
                Parsed = new Uri(rawUrl1)
            };
            var url2 = new Url
            {
                Raw = rawUrl2,
                Parsed = new Uri(rawUrl2)
            };

            // verify urls are identical
            Assert.AreEqual(
                true,
                _urlHelper.AreIdentical(
					url1.Parsed.AbsoluteUri,
					url2.Parsed.AbsoluteUri));
        }

        [Test]
        public void UrlsWithDifferentSchemeAreNotIdentical()
        {
            // create configuration without force http host patterns
            var configuration = TestData.TestData.DefaultConfiguration;
            configuration.ForceHttpHostPatterns = new List<string>();
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser(
				configuration);
			var urlHelper = new UrlHelper(
				configuration,
				urlParser,
				urlFormatter);

			// create urls
			var rawUrl1 = "http://www.test.local/url1";
            var rawUrl2 = "https://www.test.local/url1";
            var url1 = new Url
            {
                Raw = rawUrl1,
                Parsed = new Uri(rawUrl1)
            };
            var url2 = new Url
            {
                Raw = rawUrl2,
                Parsed = new Uri(rawUrl2)
            };

            // verify urls are identical using force http host pattern and one url has https scheme
            Assert.AreEqual(
                false,
                urlHelper.AreIdentical(
					url1.Parsed.AbsoluteUri,
					url2.Parsed.AbsoluteUri));
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