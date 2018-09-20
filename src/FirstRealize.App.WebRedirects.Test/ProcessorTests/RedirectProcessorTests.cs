using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using FirstRealize.App.WebRedirects.Test.Clients;
using NUnit.Framework;
using System.Collections.Generic;
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
            redirectProcessor.PreloadParsedRedirects(
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
                cyclicRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                cyclicRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri);
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
            redirectProcessor.PreloadParsedRedirects(
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

        [Test]
        public void DetectOptimizedRedirect()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser);

            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new ControlledHttpClient(),
                urlParser);

            // add redirects for optimizing
            var redirect1 = new Redirect
            {
                OldUrl = "/optimize-url1",
                NewUrl = "/optimize-url2"
            };
            var redirect2 = new Redirect
            {
                OldUrl = "/optimize-url2",
                NewUrl = "/optimize-url3"
            };
            var parsedRedirects = new List<IParsedRedirect>();
            foreach (var redirect in new[] { redirect1, redirect2 })
            {
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        redirect));
            }

            // preload redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // process redirects using redirect processor
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            // verify processed redirects has optimized redirect result
            var optimizedRedirects =
                processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.OptimizedRedirect)))
                .ToList();
            Assert.AreNotEqual(0, optimizedRedirects.Count);
            var optimizedRedirect = optimizedRedirects
                .FirstOrDefault(or => or.ParsedRedirect.OldUrl.Parsed.AbsoluteUri.Equals("http://www.test.local/optimize-url1"));
            Assert.IsNotNull(optimizedRedirect);
            var optimizedRedirectResult = optimizedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.OptimizedRedirect) &&
                r.Url.Parsed.AbsoluteUri.Equals("http://www.test.local/optimize-url3"));
            Assert.IsNotNull(optimizedRedirectResult);
        }

        [Test]
        public void DetectTooManyRedirects()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser);

            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new ControlledHttpClient(),
                urlParser);

            // add redirects
            var parsedRedirects = new List<IParsedRedirect>();
            for (var i = 1; i <= configuration.MaxRedirectCount; i++)
            {
                var redirect = new Redirect
                {
                    OldUrl = string.Format("/url{0}", i),
                    NewUrl = string.Format("/url{0}", i + 1)
                };
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        redirect));
            }

            // preload redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // process redirects using redirect processor
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            // verify processed redirects has optimized redirect result
            var redirectWithTooManyRedirects =
                processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.TooManyRedirects)))
                .ToList();
            Assert.AreEqual(1, redirectWithTooManyRedirects.Count);
            var redirectWithTooManyRedirect = redirectWithTooManyRedirects
                .FirstOrDefault();
            Assert.IsNotNull(redirectWithTooManyRedirect);
            Assert.AreEqual(
                "http://www.test.local/url1",
                redirectWithTooManyRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri);
            Assert.AreEqual(
                "http://www.test.local/url2",
                redirectWithTooManyRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri);
        }
    }
}