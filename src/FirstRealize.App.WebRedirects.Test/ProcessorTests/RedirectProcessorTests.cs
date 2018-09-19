using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Test.Clients;
using NUnit.Framework;
using System.Linq;
using System.Net;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class RedirectProcessorTests
    {
        [Test]
        public void NoCyclicRedirectsWithoutPreload()
        {
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                new[]
                { new RedirectProcessor(
                    TestData.TestData.DefaultConfiguration,
                    new ControlledHttpClient(),
                    new UrlParser())
                });

            var cyclicRedirects = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.Cyclic)))
                .ToList();
            Assert.AreEqual(0, cyclicRedirects.Count);
        }

        [Test]
        public void CanProcessCyclicRedirects()
        {
            var cyclicProcessor = new RedirectProcessor(
                new Configuration
                {
                    ForceHttp = true
                },
                new ControlledHttpClient(),
                new UrlParser());
            cyclicProcessor.PreloadRedirects(
                TestData.TestData.GetParsedRedirects());

            var processedRedirects = TestData.TestData.GetProcessedRedirects(
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

        [Test]
        public void DetectOldUrlsWithOkStatusCode()
        {
            var controlledHttpClient = new ControlledHttpClient();

            var parsedRedirects = TestData.TestData.GetParsedRedirects();
            foreach (var redirect in parsedRedirects)
            {
                controlledHttpClient.Responses[
                    redirect.OldUrl.Parsed.AbsoluteUri] = new HttpResponse
                    {
                        StatusCode = HttpStatusCode.Moved,
                        Location = redirect.NewUrl.Parsed.AbsoluteUri
                    };
            }

            controlledHttpClient.Responses[
                "http://www.test.local/new-url"] = new HttpResponse
                {
                    StatusCode = HttpStatusCode.OK
                };

            var redirectProcessor = new RedirectProcessor(
                new Configuration
                {
                    ForceHttp = true
                },
                controlledHttpClient,
                new UrlParser());

            redirectProcessor.PreloadRedirects(
                parsedRedirects);

            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            Assert.IsTrue(
                redirectProcessor.UrlsWithResponse.ContainsKey(
                "http://www.test.local/new-url")); 
        }
    }
}