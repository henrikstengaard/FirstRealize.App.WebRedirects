using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class ExcludeProcessorTests
    {
        [Test]
        public void ProcessExcludedRedirects()
        {
            var processedRedirects = TestData.GetProcessedRedirects(
                new[]
                {
                    new ExcludeProcessor(new Configuration
                    {
                        OldUrlExcludePatterns = new []{ "new-url" },
                        NewUrlExcludePatterns = new []{ "/redirect/somwhere/else" }
                    })
                });

            var excludeMatchingOldUrl = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Exclude)) &&
                pr.Redirect.OldUrl.Parsed.AbsoluteUri.Contains("new-url"));
            Assert.IsNotNull(excludeMatchingOldUrl);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                excludeMatchingOldUrl.Redirect.OldUrl.Parsed.AbsoluteUri);

            var excludeMatchingNewUrl = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Exclude)) &&
                pr.Redirect.NewUrl.Parsed.AbsoluteUri.Contains("/redirect/somwhere/else"));
            Assert.IsNotNull(excludeMatchingNewUrl);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                excludeMatchingNewUrl.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}