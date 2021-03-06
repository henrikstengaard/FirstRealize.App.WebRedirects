﻿using FirstRealize.App.WebRedirects.Core.Builders;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Validators;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.Builders
{
    [TestFixture]
    public class OutputRedirectBuilderTests
    {
        private readonly IRedirectProcessingResult _redirectProcessingResult;

        public OutputRedirectBuilderTests()
        {
            var redirectParser = new RedirectParser(
                TestData.TestData.DefaultConfiguration,
                new UrlParser(),
                new UrlFormatter());

            _redirectProcessingResult = new RedirectProcessingResult
            {
                ProcessedRedirects = new[]
                {
                    // processed redirect with cyclic redirect must be considered invalid
                    new ProcessedRedirect
                    {
                        ParsedRedirect = redirectParser.ParseRedirect(
                            new Redirect
                            {
                                OldUrl = "http://www.test1.local/url1",
                                NewUrl = "http://www.test3.local/url8"
                            }),
                        Results = new[]
                        {
                            new Result
                            {
                                Type = ResultTypes.CyclicRedirect
                            }
                        }
                    },
                    // processed redirect with not matching new url and url response status code 404 must be considered invalid
                    new ProcessedRedirect
                    {
                        ParsedRedirect = redirectParser.ParseRedirect(
                            new Redirect
                            {
                                OldUrl = "http://www.test1.local/url2",
                                NewUrl = "http://www.test2.local/url5"
                            }),
                        Results = new[]
                        {
                            new UrlResponseResult
                            {
                                Type = ResultTypes.UrlResponse,
                                Url = "http://www.test2.local/url9",
                                StatusCode = 404
                            }
                        }
                    },
                    // processed redirect with matching new url and url response status code 404 must be considered invalid
                    new ProcessedRedirect
                    {
                        ParsedRedirect = redirectParser.ParseRedirect(
                            new Redirect
                            {
                                OldUrl = "http://www.test1.local/url2",
                                NewUrl = "http://www.test2.local/url5"
                            }),
                        Results = new[]
                        {
                            new UrlResponseResult
                            {
                                Type = ResultTypes.UrlResponse,
                                Url = "http://www.test2.local/url5",
                                StatusCode = 404
                            }
                        }
                    },
                    // processed redirect with matching new url and url response status code 200 must be considered valid
                    new ProcessedRedirect
                    {
                        ParsedRedirect = redirectParser.ParseRedirect(
                            new Redirect
                            {
                                OldUrl = "http://www.test2.local/url3",
                                NewUrl = "http://www.test2.local/url9"
                            }),
                        Results = new[]
                        {
                            new UrlResponseResult
                            {
                                Type = ResultTypes.UrlResponse,
                                Url = "http://www.test2.local/url9",
                                StatusCode = 200
                            }
                        }
                    },
                    // processed redirect with not matching new url must be considered
                    // valid or invalid depending on configuration
                    new ProcessedRedirect
                    {
                        ParsedRedirect = redirectParser.ParseRedirect(
                            new Redirect
                            {
                                OldUrl = "http://www.test2.local/url4",
                                NewUrl = "http://www.test2.local/url9"
                            }),
                        Results = new[]
                        {
                            new UrlResponseResult
                            {
                                Type = ResultTypes.UrlResponse,
                                Url = "http://www.test2.local/url10",
                                StatusCode = 200
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public void BuildOutputRedirects()
        {
            // create and build filtered redirect report
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlFormatter = new UrlFormatter();
            var urlParser = new UrlParser();
            var urlHelper = new UrlHelper(
                configuration,
                urlParser,
                urlFormatter);
            var processedRedirectValidator = new ProcessedRedirectValidator(
                    configuration,
                    urlHelper);
            var outputRedirectBuilder = new OutputRedirectBuilder
                (processedRedirectValidator);

            var validOutputRedirects = _redirectProcessingResult.ProcessedRedirects
                .Select(x => outputRedirectBuilder.Build(x))
                .Where(x => x.ValidMatchingOriginalNewUrl || x.ValidNotMatchingOriginalNewUrl)
                .ToList();

            // verify valid output redirects
            Assert.AreEqual(2, validOutputRedirects.Count);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                validOutputRedirects[0].OldUrl);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                validOutputRedirects[0].NewUrl);
            Assert.AreEqual(
                "http://www.test2.local/url4",
                validOutputRedirects[1].OldUrl);
            Assert.AreEqual(
                "http://www.test2.local/url10",
                validOutputRedirects[1].NewUrl);
        }
    }
}