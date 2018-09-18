using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class DuplicateProcessorTests
    {
        [Test]
        public void CanProcessDuplicatedRedirects()
        {
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                new[] { new DuplicateProcessor() });

            var duplicatedRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Duplicated)));
            Assert.IsNotNull(duplicatedRedirect);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                duplicatedRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}