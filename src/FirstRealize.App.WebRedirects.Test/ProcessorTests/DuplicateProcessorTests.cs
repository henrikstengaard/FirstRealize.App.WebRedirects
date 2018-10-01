using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class DuplicateProcessorTests
    {
        [Test]
        public void DetectDuplicatedOfFirstRedirects()
        {
            // get processed redirects
            var configuration = TestData.TestData.DefaultConfiguration;
            configuration.DuplicateOldUrlStrategy = DuplicateUrlStrategy.KeepFirst;
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser();
			var urlHelper = new UrlHelper(
				configuration,
				urlParser,
				urlFormatter);
			var processedRedirects = TestData.TestData.GetProcessedRedirects(
                configuration,
                new[] { new DuplicateProcessor(
                    TestData.TestData.DefaultConfiguration,
                    urlHelper) });

            // verify duplicate of first redirects are detected
            var duplicateOfFirstRedirects = processedRedirects
                .Where(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.DuplicateOfFirst)))
                    .ToList();
            Assert.AreEqual(
                1,
                duplicateOfFirstRedirects.Count);
            var duplicateOfFirstRedirect =
                duplicateOfFirstRedirects.FirstOrDefault();
            Assert.IsNotNull(duplicateOfFirstRedirect);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                duplicateOfFirstRedirect.ParsedRedirect.NewUrl.Formatted);

            // verify no duplicate of last redirects are detected
            var duplicateOfLastRedirects = processedRedirects
                .Where(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.DuplicateOfLast)))
                    .ToList();
            Assert.AreEqual(
                0,
                duplicateOfLastRedirects.Count);
        }

        [Test]
        public void DetectDuplicatedOfLastRedirects()
        {
            // get processed redirects
            var configuration = TestData.TestData.DefaultConfiguration;
            configuration.DuplicateOldUrlStrategy = DuplicateUrlStrategy.KeepLast;
			var urlFormatter = new UrlFormatter();
			var urlParser = new UrlParser();
			var urlHelper = new UrlHelper(
				configuration,
				urlParser,
				urlFormatter);
			var processedRedirects = TestData.TestData.GetProcessedRedirects(
                configuration,
                new[] { new DuplicateProcessor(
                    TestData.TestData.DefaultConfiguration,
                    urlHelper) });

            // verify duplicate of last redirects are detected
            var duplicateOfLastRedirects = processedRedirects
                .Where(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.DuplicateOfLast)))
                    .ToList();
            Assert.AreEqual(
                1,
                duplicateOfLastRedirects.Count);
            var duplicateOfLastRedirect =
                duplicateOfLastRedirects.FirstOrDefault();
            Assert.IsNotNull(duplicateOfLastRedirect);
            Assert.AreEqual(
                "http://www.test.local/another/path",
                duplicateOfLastRedirect.ParsedRedirect.NewUrl.Formatted);

            // verify no duplicate of first redirects are detected
            var duplicateOfFirstRedirects = processedRedirects
                .Where(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.DuplicateOfFirst)))
                    .ToList();
            Assert.AreEqual(
                0,
                duplicateOfFirstRedirects.Count);
        }
    }
}