﻿using FirstRealize.App.WebRedirects.Core.Builders;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Reports;
using FirstRealize.App.WebRedirects.Core.Validators;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class OutputRedirectReportTests
    {
        private readonly IRedirectProcessingResult _redirectProcessingResult;

        public OutputRedirectReportTests()
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
        public void BuildRedirectReportMatchingNewUrl()
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
            var outputRedirectReport = new OutputRedirectReport(
                outputRedirectBuilder,
                false);
            outputRedirectReport.Build(_redirectProcessingResult);

            // verify filtered redirect records are build
            var records = outputRedirectReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(1, records.Count);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                records[0].OldUrl);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                records[0].NewUrl);
        }

        [Test]
        public void BuildRedirectReportNotMatchingNewUrl()
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
            var outputRedirectReport = new OutputRedirectReport(
                outputRedirectBuilder,
                true);
            outputRedirectReport.Build(_redirectProcessingResult);

            // verify filtered redirect records are build
            var records = outputRedirectReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(2, records.Count);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                records[0].OldUrl);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                records[0].NewUrl);
            Assert.AreEqual(
                "http://www.test2.local/url4",
                records[1].OldUrl);
            Assert.AreEqual(
                "http://www.test2.local/url10",
                records[1].NewUrl);
        }
    }
}