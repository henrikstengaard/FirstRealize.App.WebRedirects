using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class ExcludeProcessorTests
    {
        [Test]
        public void CanProcessExcludedRedirects()
        {
            var configuration = new Configuration
            {
                OldUrlExcludePatterns = new[] { "new-url" },
                NewUrlExcludePatterns = new[] { "/redirect/somwhere/else" }
            };
            var excludeProcessor = new ExcludeProcessor(
                configuration);
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                new[]
                {
                    excludeProcessor
                });

            var excludedRedirectsMatchingOldUrl = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.ExcludedRedirect)) &&
                pr.ParsedRedirect.OldUrl.Formatted.Contains("new-url"))
                .ToList();
            Assert.AreEqual(2, excludedRedirectsMatchingOldUrl.Count);
            foreach(var excludedRedirect in excludedRedirectsMatchingOldUrl)
            {
                Assert.AreEqual(
                    "http://www.test.local/new-url",
                    excludedRedirect.ParsedRedirect.OldUrl.Formatted);
            }

            var excludedRedirectsMatchingNewUrl = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.ExcludedRedirect)) &&
                pr.ParsedRedirect.NewUrl.Formatted.Contains("/redirect/somwhere/else"))
                .ToList();
            var excludedRedirectMatchingNewUrl = excludedRedirectsMatchingNewUrl
                .FirstOrDefault();
            Assert.AreEqual(1, excludedRedirectsMatchingNewUrl.Count);
            Assert.IsNotNull(excludedRedirectMatchingNewUrl);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                excludedRedirectMatchingNewUrl.ParsedRedirect.NewUrl.Formatted);
        }
    }
}