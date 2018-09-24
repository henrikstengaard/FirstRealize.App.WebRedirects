﻿using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Reports;
using FirstRealize.App.WebRedirects.Core.Validators;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class RedirectSummaryReportTests
    {
        public void BuildRedirectSummaryReport()
        {
            var oldUrlRaw = "http://www.test.local/old";
            var newUrlRaw = "http://www.test.local/new";
            var oldUrlParsed = new Uri(oldUrlRaw);
            var newUrlParsed = new Uri(newUrlRaw);

            var redirectProcessingResult = new RedirectProcessingResult
            {
                ProcessedRedirects = new[]
                {
                    new ProcessedRedirect
                    {
                        ParsedRedirect = new ParsedRedirect
                        {
                            OldUrl = new Url
                            {
                                Raw = oldUrlRaw,
                                Parsed = oldUrlParsed
                            },
                            NewUrl = new Url
                            {
                                Raw = newUrlRaw,
                                Parsed = newUrlParsed
                            }
                        },
                        Results = new []
                        {
                            new Result
                            {
                                Type = ResultTypes.ExcludedRedirect,
                                Message = ResultTypes.ExcludedRedirect,
                                Url = new Url
                                {
                                    Raw = newUrlRaw,
                                    Parsed = newUrlParsed
                                }
                            },
                            new Result
                            {
                                Type = ResultTypes.DuplicateOfFirst,
                                Message = ResultTypes.DuplicateOfFirst,
                                Url = new Url
                                {
                                    Raw = newUrlRaw,
                                    Parsed = newUrlParsed
                                }
                            },
                            new Result
                            {
                                Type = ResultTypes.DuplicateOfLast,
                                Message = ResultTypes.DuplicateOfLast,
                                Url = new Url
                                {
                                    Raw = newUrlRaw,
                                    Parsed = newUrlParsed
                                }
                            }
                        }
                    }
                }
            };

            // create and build redirect summary report
            var redirectSummaryReport = new RedirectSummaryReport(
                new ProcessedRedirectValidator(
                    new UrlHelper(
                        TestData.TestData.DefaultConfiguration)));
            redirectSummaryReport.Build(redirectProcessingResult);

            // verify redirect summary records are build
            var records = redirectSummaryReport
                .GetRecords()
                .ToList();
            Assert.AreNotEqual(0, records.Count);
        }
    }
}