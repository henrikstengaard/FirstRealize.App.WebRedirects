using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class IdenticalProcessorTests
    {
        private readonly IUrlHelper _urlHelper;

        public IdenticalProcessorTests()
        {
            _urlHelper = new UrlHelper(
                TestData.TestData.DefaultConfiguration);
        }

        [Test]
        public void DetectIdenticalRedirects()
        {
            // identical redirect
            var rawUrl = "http://www.test.local";
            var parsedUrl = new Uri(rawUrl);
            var parsedRedirects = new[]
            {
                new ParsedRedirect
                {
                    OldUrl = new Url
                    {
                        Raw = rawUrl,
                        Parsed = parsedUrl
                    },
                    NewUrl = new Url
                    {
                        Raw = rawUrl,
                        Parsed = parsedUrl
                    }
                }
            };

            // verify parsed redirect is identical
            Assert.AreEqual(
                true,
                _urlHelper.AreIdentical(
                    parsedRedirects[0].OldUrl,
                    parsedRedirects[0].NewUrl));

            // processed redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { new IdenticalProcessor(
                    _urlHelper) });

            // verify identical redirect detected
            var identicalRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.IdenticalResult)));
            Assert.IsNotNull(identicalRedirect);
        }
    }
}