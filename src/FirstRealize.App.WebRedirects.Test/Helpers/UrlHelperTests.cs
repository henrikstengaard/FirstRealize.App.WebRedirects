using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Test.Helpers
{
    [TestFixture]
    public class UrlHelperTests
    {
        private readonly IUrlHelper _urlHelper;

        public UrlHelperTests()
        {
            // create url helper
            _urlHelper = new UrlHelper(
                TestData.TestData.DefaultConfiguration);
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
                _urlHelper.IsHttpsRedirect(url1, url2));
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
                _urlHelper.IsHttpsRedirect(url1, url2));
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
                _urlHelper.AreIdentical(url1, url2));
        }

        [Test]
        public void UrlsWithDifferentSchemeAreNotIdentical()
        {
            // create configuration without force http host patterns
            var configuration = TestData.TestData.DefaultConfiguration;
            configuration.ForceHttpHostPatterns = new List<string>();
            var urlHelper = new UrlHelper(
                configuration);

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
                urlHelper.AreIdentical(url1, url2));
        }
    }
}