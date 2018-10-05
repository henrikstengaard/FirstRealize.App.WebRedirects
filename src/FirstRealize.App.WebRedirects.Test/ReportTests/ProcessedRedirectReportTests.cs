using FirstRealize.App.WebRedirects.Core.Builders;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Formatters;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Parsers;
using FirstRealize.App.WebRedirects.Core.Reports;
using FirstRealize.App.WebRedirects.Core.Validators;
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

            var configuration = TestData.TestData.DefaultConfiguration;
            var parsedRedirect = TestData.TestData.GetParsedRedirects(
                configuration,
                new[]
                {
                    new Redirect
                    {
                        OldUrl = oldUrlRaw,
                        NewUrl = newUrlRaw
                    }
                })
                .FirstOrDefault();

            var redirectProcessingResult = new RedirectProcessingResult
            {
                ProcessedRedirects = new[]
                {
                    new ProcessedRedirect
                    {
                        ParsedRedirect = parsedRedirect,
                        Results = new []
                        {
                            new Result
                            {
                                Type = ResultTypes.ExcludedRedirect,
                                Message = ResultTypes.ExcludedRedirect,
                                Url = newUrlRaw
                            },
                            new Result
                            {
                                Type = ResultTypes.DuplicateOfFirst,
                                Message = ResultTypes.DuplicateOfFirst,
                                Url = newUrlRaw
                            },
                            new Result
                            {
                                Type = ResultTypes.DuplicateOfLast,
                                Message = ResultTypes.DuplicateOfLast,
                                Url = newUrlRaw
                            }
                        }
                    }
                }
            };

            // create and build processed redirect report
            var urlHelper = new UrlHelper(
                configuration,
                new UrlParser(),
                new UrlFormatter());
            var processedRedirectValidator =
                new ProcessedRedirectValidator(
                    configuration,
                    urlHelper);
            var outputRedirectBuilder = new OutputRedirectBuilder
                (processedRedirectValidator);
            var processedRedirectReport = new ProcessedRedirectReport(
                outputRedirectBuilder);
            processedRedirectReport.Build(redirectProcessingResult);

            // verify processed redirect records are build
            var records = processedRedirectReport
                .GetRecords()
                .ToList();
            Assert.AreEqual(1, records.Count);
            Assert.AreEqual(
                oldUrlRaw,
                records[0].OriginalOldUrl);
            Assert.AreEqual(
                newUrlRaw,
                records[0].OriginalNewUrl);
            Assert.AreEqual(
                oldUrlRaw,
                records[0].ParsedOldUrl);
            Assert.AreEqual(
                newUrlRaw,
                records[0].ParsedNewUrl);

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