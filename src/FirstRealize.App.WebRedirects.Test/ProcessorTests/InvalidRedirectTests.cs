using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class InvalidRedirectTests
    {
        [Test]
        public void DetectInvalidRedirects()
        {
            // invalid redirect
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
                        Raw = ""
                    }
                }
            };

            // verify parsed redirect is invalid
            Assert.AreEqual(
                false,
                parsedRedirects[0].IsValid);

            // processed redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { new InvalidProcessor() });

            // verify invalid redirect is detected
            var invalidRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.InvalidResult)));
            Assert.IsNotNull(invalidRedirect);
        }
    }
}