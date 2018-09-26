﻿using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Reports;
using FirstRealize.App.WebRedirects.Core.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class FilteredRedirectReportTests
    {
        private readonly IRedirectProcessingResult _redirectProcessingResult;

        public FilteredRedirectReportTests()
        {
            _redirectProcessingResult = new RedirectProcessingResult
            {
                ProcessedRedirects = new[]
                {
                    // processed redirect with cyclic redirect must be considered invalid
                    new ProcessedRedirect
                    {
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Parsed = new Uri("http://www.test1.local/url1")
                            },
                            NewUrl = new Url
                            {
                                Parsed = new Uri("http://www.test3.local/url8")
                            }
                        },
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
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Parsed = new Uri("http://www.test1.local/url2")
                            },
                            NewUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url5")
                            }
                        },
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
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Parsed = new Uri("http://www.test1.local/url2")
                            },
                            NewUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url5")
                            }
                        },
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
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url3")
                            },
                            NewUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url9")
                            }
                        },
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
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url4")
                            },
                            NewUrl = new Url
                            {
                                Parsed = new Uri("http://www.test2.local/url9")
                            }
                        },
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
        public void BuildFilteredRedirectReport()
        {
            // create and build filtered redirect report
            var configuration =
                TestData.TestData.DefaultConfiguration;
            var urlFormatter = new UrlFormatter();
            var urlParser = new UrlParser(
                configuration);
            var urlHelper = new UrlHelper(
                configuration,
                urlParser,
                urlFormatter);
            var filteredRedirectReport = new FilteredRedirectReport(
                new ProcessedRedirectValidator(
                    configuration,
                    urlHelper),
                true);
            filteredRedirectReport.Build(_redirectProcessingResult);

            // verify filtered redirect records are build
            var records = filteredRedirectReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(2, records.Count);
            Assert.AreEqual(
                "http://www.test2.local/url3",
                records[0].OldUrlResult);
            Assert.AreEqual(
                "http://www.test2.local/url9",
                records[0].NewUrlResult);
            Assert.AreEqual(
                "http://www.test2.local/url4",
                records[1].OldUrlResult);
            Assert.AreEqual(
                "http://www.test2.local/url10",
                records[1].NewUrlResult);
        }
    }
}