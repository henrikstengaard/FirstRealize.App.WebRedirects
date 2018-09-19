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
            // process redirects
            var processedRedirects = 
                TestData.TestData.GetProcessedRedirects(
                new[]
                { new RedirectProcessor(
                    TestData.TestData.DefaultConfiguration,
                    new ControlledHttpClient(),
                    new UrlParser())
                });

            // verify no cyclic redirects are detected
            var cyclicRedirects = processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.CyclicRedirect)))
                .ToList();
            Assert.AreEqual(0, cyclicRedirects.Count);
        }

        [Test]
        public void DetectCyclicRedirect()
        {
            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                new Configuration
                {
                    ForceHttp = true
                },
                new ControlledHttpClient(),
                new UrlParser());

            // parsed redirects
            var redirects = TestData.TestData.GetParsedRedirects();

            // preload redirects
            redirectProcessor.PreloadRedirects(
                redirects);

            // process redirects using redirect processor
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                redirects,
                new[] { redirectProcessor });

            // verify cyclic redirect is detected
            var cyclicRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.CyclicRedirect)));
            Assert.IsNotNull(cyclicRedirect);
            Assert.AreEqual(
                "http://www.test.local/example/path",
                cyclicRedirect.Redirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                cyclicRedirect.Redirect.NewUrl.Parsed.AbsoluteUri);
        }

        [Test]
        public void DetectUrlWithResponse()
        {
            // create controlled http client
            var controlledHttpClient = new ControlledHttpClient();

            // parsed redirects
            var parsedRedirects = 
                TestData.TestData.GetParsedRedirects();

            // add moved response for parsed redirects
            foreach (var redirect in parsedRedirects)
            {
                controlledHttpClient.Responses[
                    redirect.OldUrl.Parsed.AbsoluteUri] = new HttpResponse
                    {
                        StatusCode = HttpStatusCode.Moved,
                        Location = redirect.NewUrl.Parsed.AbsoluteUri
                    };
            }

            // override redirect old url with ok response
            controlledHttpClient.Responses[
                "http://www.test.local/new-url"] = new HttpResponse
                {
                    StatusCode = HttpStatusCode.OK
                };

            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                new Configuration
                {
                    ForceHttp = true
                },
                controlledHttpClient,
                new UrlParser());

            // preload redirects
            redirectProcessor.PreloadRedirects(
                parsedRedirects);

            // process redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            // verify redirect processor detects overridden url with response
            var urlWithResponse = redirectProcessor.Results
                .FirstOrDefault(x => x.Type.Equals(ResultTypes.UrlWithResponse));
            Assert.IsNotNull(urlWithResponse);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                urlWithResponse.Url.Parsed.AbsoluteUri); 
        }
    }
}