using FirstRealize.App.WebRedirects.Core.Clients;
using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Processors;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ProcessorTests
{
    [TestFixture]
    public class RedirectProcessorTests
    {
        [Test]
        public void HttpsRedirectDoesntReturnCyclicRedirect()
        {
            // create redirect processor
            var configuration = new Configuration
            {
                DefaultUrl = TestData.TestData.DefaultHost,
                ForceHttpHostPatterns = new[]
                {
                    "www\\.test\\.local"
                }
            };

            // create url and redirect parser
            var urlFormatter = new UrlFormatter();
            var urlParser = new UrlParser();
            var urlHelper = new UrlHelper(
                configuration,
                urlParser,
                urlFormatter);
            var redirectParser = new RedirectParser(
                configuration,
                urlParser,
                urlFormatter);

            // create redirect processor
            var testHttpClient =
                new TestHttpClient();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                urlHelper,
                testHttpClient,
                urlParser);

            // parse redirects
            var redirects = new[] {
                new Redirect
                {
                    OldUrl = "http://www.test.local/url1",
                    NewUrl = "https://www.test.local/url1"
                },
                new Redirect
                {
                    OldUrl = "https://www.test.local/url1",
                    NewUrl = "https://www.test.local/url2"
                },
                new Redirect
                {
                    OldUrl = "https://www.test.local/url2",
                    NewUrl = "https://www.test.local/url3"
                }
            };

            // parse redirects
            var parsedRedirects = new List<IParsedRedirect>();
            foreach (var redirect in redirects)
            {
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        redirect));
            }

            // preload parsed redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // process redirects using redirect processor
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            // verify cyclic redirect is not detected
            var cyclicRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.CyclicRedirect)));
            Assert.IsNull(cyclicRedirect);

            // verify optimized redirect is detected
            var optimizedRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(
                    r => r.Type.Equals(ResultTypes.OptimizedRedirect)));
            Assert.IsNotNull(optimizedRedirect);
        }

        [Test]
        public void CyclicRedirectsNotDetectedWithoutPreload()
        {
            // create redirect processor
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                new TestHttpClient(),
                urlParser
            );

            // process redirects
            var processedRedirects =
                TestData.TestData.GetProcessedRedirects(
                new[]
                {
                    redirectProcessor
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
            var configuration = new Configuration
            {
                ForceHttpHostPatterns = new[]
                {
                    "www\\.test\\.local"
                }
            };
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                new TestHttpClient(),
                urlParser);

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
                cyclicRedirect.ParsedRedirect.OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                cyclicRedirect.ParsedRedirect.NewUrl.Formatted);
        }

        [Test]
        public void DetectOnlyCyclicRedirect()
        {
            // create redirect processor
            var configuration = new Configuration
            {
                ForceHttpHostPatterns = new[]
                {
                    "www\\.test\\.local"
                }
            };
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                new TestHttpClient(),
                urlParser);

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
            var optimizedRedirect = processedRedirects
                .FirstOrDefault(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.OptimizedRedirect)));
            Assert.IsNull(optimizedRedirect);
        }

        [Test]
        public void DetectUrlResponse()
        {
            // create test http client
            var testHttpClient = new TestHttpClient();

            // parsed redirects
            var parsedRedirects =
                TestData.TestData.GetParsedRedirects();

            // add moved response for parsed redirects
            foreach (var redirect in parsedRedirects)
            {
                testHttpClient.Responses[
                    redirect.OldUrl.Formatted] = new HttpResponse
                    {
                        StatusCode = 301,
                        Headers = new Dictionary<string, string>
                        {
                            { "Location", redirect.NewUrl.Formatted }
                        }
                    };
            }

            // override redirect old url with ok response
            testHttpClient.Responses[
                "http://www.test.local/new-url"] = new HttpResponse
                {
                    StatusCode = 200
                };

            // create redirect processor
            var configuration = new Configuration
            {
                DefaultUrl = TestData.TestData.DefaultHost,
                ForceHttpHostPatterns = new[]
                {
                    "www\\.test\\.local"
                }
            };
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                testHttpClient,
                urlParser);

            // preload redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // process redirects
            var processedRedirects = TestData.TestData.GetProcessedRedirects(
                parsedRedirects,
                new[] { redirectProcessor });

            // verify redirect processor detects overridden url with response
            var urlResponse = redirectProcessor.Results
                .FirstOrDefault(x => x.Type.Equals(ResultTypes.UrlResponse));
            Assert.IsNotNull(urlResponse);
            Assert.AreEqual(
                "http://www.test.local/new-url",
                urlResponse.Url);
        }

        [Test]
        public void DetectOptimizedRedirect()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser,
                urlFormatter);

            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                new TestHttpClient(),
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
                .FirstOrDefault(or => or.ParsedRedirect.OldUrl.Formatted.Equals("http://www.test.local/optimize-url1"));
            Assert.IsNotNull(optimizedRedirect);
            var optimizedRedirectResult = optimizedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.OptimizedRedirect) &&
                r.Url != null && r.Url.Equals("http://www.test.local/optimize-url3"));
            Assert.IsNotNull(optimizedRedirectResult);
        }

        [Test]
        public void DetectTooManyRedirects()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser,
                urlFormatter);

            // create redirect processor
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                new TestHttpClient(),
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

            // verify processed redirects has too many redirects result
            var redirectWithTooManyRedirects =
                processedRedirects
                .Where(pr => pr.Results.Any(r => r.Type.Equals(ResultTypes.TooManyRedirects)))
                .ToList();
            Assert.AreEqual(
                1,
                redirectWithTooManyRedirects.Count);
            var redirectWithTooManyRedirect = redirectWithTooManyRedirects
                .FirstOrDefault();
            Assert.IsNotNull(redirectWithTooManyRedirect);
            Assert.AreEqual(
                "http://www.test.local/url1",
                redirectWithTooManyRedirect.ParsedRedirect.OldUrl.Formatted);
            Assert.AreEqual(
                "http://www.test.local/url2",
                redirectWithTooManyRedirect.ParsedRedirect.NewUrl.Formatted);
        }

        [Test]
        public void RedirectProcessorCachesResponse()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser,
                urlFormatter);

            // create redirect processor
            var testHttpClient =
                new TestHttpClient();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                testHttpClient,
                urlParser);

            // create and parse redirects
            var redirects = new List<IRedirect>
            {
                new Redirect
                {
                    OldUrl = "/url1",
                    NewUrl = "/url3"
                },
                new Redirect
                {
                    OldUrl = "/url2",
                    NewUrl = "/url3"
                }
            };
            var parsedRedirects = new List<IParsedRedirect>();
            foreach (var redirect in redirects)
            {
                parsedRedirects.Add(
                    redirectParser.ParseRedirect(
                        redirect));
            }

            // preload parsed redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // verify controlled http client doesn't have any responses
            Assert.AreEqual(
                0,
                testHttpClient.Responses.Count);

            // process redirects and verify responses are cached by overriding responses
            UrlResponseResult urlResponseResult = null;
            var processedRedirects = new List<IProcessedRedirect>();
            foreach (var parsedRedirect in parsedRedirects)
            {
                var processedRedirect = new ProcessedRedirect
                {
                    ParsedRedirect = parsedRedirect
                };

                redirectProcessor.Process(
                    processedRedirect);

                // get url response result, if url response result is null and
                // controlled http client has a response for old url
                if (urlResponseResult == null &&
                    testHttpClient.Responses.ContainsKey(
                    parsedRedirect.NewUrl.Formatted))
                {
                    urlResponseResult = processedRedirect.Results
                        .FirstOrDefault(r => r.Type.Equals(
                            ResultTypes.UrlResponse)) as UrlResponseResult;
                }
                else
                {
                    // override response with forbidden status code
                    testHttpClient.Responses[
                        parsedRedirect.NewUrl.Formatted] =
                        new HttpResponse
                        {
                            StatusCode = 401
                        };
                }
            }

            // verify url response result for /url3 has status code ok and not forbidden
            Assert.IsNotNull(
                urlResponseResult);
            Assert.AreEqual(
                "http://www.test.local/url3",
                urlResponseResult.Url);
            Assert.AreEqual(
                404,
                urlResponseResult.StatusCode
                );
        }

        [Test]
        public void RedirectProcessorHandlesLowercasedRedirects()
        {
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlParser = new UrlParser();
            var urlFormatter = new UrlFormatter();
            var redirectParser = new RedirectParser(
                configuration,
                urlParser,
                urlFormatter);

            // create redirect processor
            var testHttpClient =
                new TestHttpClient();
            var redirectProcessor = new RedirectProcessor(
                configuration,
                new UrlHelper(
                    configuration,
                    urlParser,
                    urlFormatter),
                testHttpClient,
                urlParser);

            // parse redirects with uppercased old url
            var parsedRedirects = TestData.TestData.GetParsedRedirects(
                configuration,
                new[]
                {
                    new Redirect
                    {
                        OldUrl = "/Url1",
                        NewUrl = "/url2"
                    }
                })
                .ToList();

            // simulate uppercased url redirects to lowercased url
            testHttpClient.Responses[
                parsedRedirects[0].OldUrl.Formatted] =
                new HttpResponse
                {
                    StatusCode = 301,
                    Headers = new Dictionary<string, string>
                    {
                        { "Location", "/url1" }
                    }
                };

            // preload parsed redirects
            redirectProcessor.PreloadParsedRedirects(
                parsedRedirects);

            // process redirects
            var processedRedirect = new ProcessedRedirect
            {
                ParsedRedirect = parsedRedirects[0]
            };
            redirectProcessor.Process(
                processedRedirect);

            // verify
            var urlResponseResult = processedRedirect
                .Results
                .FirstOrDefault(x => x.Type == ResultTypes.UrlResponse);
            Assert.IsNotNull(urlResponseResult);
            Assert.AreEqual(
                "http://www.test.local/url2",
                urlResponseResult.Url);
        }
    }
}