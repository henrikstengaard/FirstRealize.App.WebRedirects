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
                parsedRedirects[0].IsIdentical);

            // processed redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { new IdenticalProcessor() });

            // verify identical redirect detected
            var identicalRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.IdenticalResult)));
            Assert.IsNotNull(identicalRedirect);
        }
    }
}