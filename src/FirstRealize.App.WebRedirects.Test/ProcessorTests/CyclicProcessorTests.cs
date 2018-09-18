using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class CyclicProcessorTests
    {
        [Test]
        public void CyclicProcessorWithoutPreloadReturnsNone()
        {
            var processedRedirects = TestData.GetProcessedRedirects(
                new[]
                { new CyclicProcessor(
                    new Configuration
                    {
                        ForceHttp = true
                    })
                });

            var cyclicRedirects = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Cyclic)))
                .ToList();
            Assert.AreEqual(0, cyclicRedirects.Count);
        }

        [Test]
        public void CanProcessCyclicRedirects()
        {
            var cyclicProcessor = new CyclicProcessor(
                new Configuration
                {
                    ForceHttp = true
                });
            cyclicProcessor.PreloadRedirects(TestData.GetParsedRedirects());

            var processedRedirects = TestData.GetProcessedRedirects(
                new[] { cyclicProcessor });

            var cyclicRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Cyclic)));
            Assert.IsNotNull(cyclicRedirect);
            Assert.AreEqual(
                "http://www.test.local/example/path",
                cyclicRedirect.Redirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                cyclicRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}