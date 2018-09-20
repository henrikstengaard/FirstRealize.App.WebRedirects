using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Reports;
using NUnit.Framework;
using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ReportTests
{
    [TestFixture]
    public class ProcessedRedirectReportTests
    {
        [Test]
        public void BuildProcessedRedirectReport()
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

            // create and build processed redirect report
            var processedRedirectReport = new ProcessedRedirectReport();
            processedRedirectReport.Build(redirectProcessingResult);

            // verify processed redirect records are build
            var records = processedRedirectReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(1, records.Count);
            Assert.AreEqual(
                oldUrlRaw,
                records[0].OldUrlRaw);
            Assert.AreEqual(
                newUrlRaw,
                records[0].NewUrlRaw);
            Assert.AreEqual(
                oldUrlRaw,
                records[0].OldUrlParsed);
            Assert.AreEqual(
                newUrlRaw,
                records[0].NewUrlParsed);

            Assert.AreEqual(
                true,
                records[0].Valid);

            Assert.AreEqual(
                3,
                records[0].ResultCount);
            Assert.AreEqual(
                string.Join(",", new []
                {
                    ResultTypes.DuplicateOfFirst,
                    ResultTypes.DuplicateOfLast,
                    ResultTypes.ExcludedRedirect
                }),
                records[0].ResultTypes);

            Assert.AreEqual(
                true,
                records[0].ExcludedRedirect);
            Assert.AreEqual(
                ResultTypes.ExcludedRedirect,
                records[0].ExcludedRedirectMessage);
            Assert.AreEqual(
                newUrlRaw,
                records[0].ExcludedRedirectUrl);

            Assert.AreEqual(
                true,
                records[0].DuplicateOfFirst);
            Assert.AreEqual(
                ResultTypes.DuplicateOfFirst,
                records[0].DuplicateOfFirstMessage);
            Assert.AreEqual(
                newUrlRaw,
                records[0].DuplicateOfFirstUrl);

            Assert.AreEqual(
                true,
                records[0].DuplicateOfLast);
            Assert.AreEqual(
                ResultTypes.DuplicateOfLast,
                records[0].DuplicateOfLastMessage);
            Assert.AreEqual(
                newUrlRaw,
                records[0].DuplicateOfLastUrl);
        }
    }
}