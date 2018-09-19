using FirstRealize.App.WebRedirects.Core.Models.Results;
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
            // processed redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                new[] { new DuplicateProcessor() });

            // verify duplicate of first old url detected
            var duplicateOfFirstRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.DuplicateOfFirst)));
            Assert.IsNotNull(duplicateOfFirstRedirect);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                duplicateOfFirstRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);
        }

        [Test]
        public void DetectDuplicatedOfLastRedirects()
        {
            // processed redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                new[] { new DuplicateProcessor() });

            // verify duplicate of last old url detected
            var duplicateOfLastRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.DuplicateOfLast)));
            Assert.IsNotNull(duplicateOfLastRedirect);
            Assert.AreEqual(
                "http://www.test.local/another/path",
                duplicateOfLastRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}