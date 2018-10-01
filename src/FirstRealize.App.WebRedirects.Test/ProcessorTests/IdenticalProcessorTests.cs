using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class IdenticalProcessorTests
    {
        private readonly IUrlParser _urlParser;
        private readonly IUrlHelper _urlHelper;

        public IdenticalProcessorTests()
        {
			var configuration =
				TestData.TestData.DefaultConfiguration;
			var urlFormatter = new UrlFormatter();
			_urlParser = new UrlParser();
			_urlHelper = new UrlHelper(
                configuration,
				_urlParser,
				urlFormatter);
        }

        [Test]
        public void DetectIdenticalRedirects()
        {
            // identical redirect
            var parsedRedirects = TestData.TestData.GetParsedRedirects(
                new[]
                {
                    new Redirect{
                        OldUrl = "http://www.test.local",
                        NewUrl = "http://www.test.local"
                    }
                })
                .ToList();

            // verify parsed redirect is identical
            Assert.AreEqual(
                true,
                _urlHelper.AreIdentical(
                    parsedRedirects[0].OldUrl.Formatted,
                    parsedRedirects[0].NewUrl.Formatted));

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