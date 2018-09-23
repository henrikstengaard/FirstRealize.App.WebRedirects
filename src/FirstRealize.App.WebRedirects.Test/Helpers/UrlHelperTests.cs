using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using NUnit.Framework;
using System;

namespace FirstRealize.App.WebRedirects.Test.Helpers
{
    [TestFixture]
    public class UrlHelperTests
    {
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

            // create url helper
            var urlHelper = new UrlHelper(
                TestData.TestData.DefaultConfiguration);

            // verify urls is https redirect
            Assert.AreEqual(
                true,
                urlHelper.IsHttpsRedirect(url1, url2));
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

            // create url helper
            var urlHelper = new UrlHelper(
                TestData.TestData.DefaultConfiguration);

            // verify urls is not https redirect with only http scheme
            Assert.AreEqual(
                false,
                urlHelper.IsHttpsRedirect(url1, url2));
        }
    }
}