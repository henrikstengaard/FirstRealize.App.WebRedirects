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
        public void ProcessDuplicateRedirects()
        {
            var processedRedirects = TestData.GetProcessedRedirects(
                new[] { new DuplicateProcessor() });

            var duplicate = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Duplicate)));
            Assert.IsNotNull(duplicate);
            Assert.AreEqual(
                "http://www.test.local/redirect/somwhere/else",
                duplicate.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}