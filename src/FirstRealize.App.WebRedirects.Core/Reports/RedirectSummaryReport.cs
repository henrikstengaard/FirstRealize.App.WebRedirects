using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Validators;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class RedirectSummaryReport : ReportBase<RedirectSummaryReportRecord>
    {
        private readonly IProcessedRedirectValidator _processedRedirectValidator;
        private readonly IList<RedirectSummaryReportRecord> _redirectSummaryReportRecords;

        public RedirectSummaryReport(
            IProcessedRedirectValidator processedRedirectValidator)
        {
            _processedRedirectValidator = processedRedirectValidator;
            _redirectSummaryReportRecords =
                new List<RedirectSummaryReportRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            // parsed redirects
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ParsedRedirects
                    .Count()
                    .ToString(),
                    RedirectSummaryType = "parsed redirects"
                });

            // processors
            var processors = redirectProcessingResult
                .Processors
                .Distinct()
                .OrderBy(p => p.Name)
                .ToList();

            // processors used
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = processors.Count.ToString(),
                    RedirectSummaryType = "processors used"
                });

            // results from processor
            foreach (var processor in processors)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = "1",
                        RedirectSummaryType = string.Format(
                            "processor '{0}'",
                            processor.Name)
                    });
            }

            // get old url domains
            var oldUrlDomains =
                redirectProcessingResult.ProcessedRedirects
                .Where(
                    pr => pr.ParsedRedirect.IsValid)
                .Select(
                    pr => pr.ParsedRedirect.OldUrl.Parsed.Host.ToLower())
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

            // domains in parsed and valid old urls
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount =
                    oldUrlDomains.Count.ToString(),
                    RedirectSummaryType = "domains in parsed and valid old urls"
                });

            // parsed and valid old urls has domain
            foreach (var oldUrlDomain in oldUrlDomains)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = redirectProcessingResult
                        .ParsedRedirects
                        .Count(pr => pr.IsValid && pr.OldUrl.Parsed.Host.Equals(oldUrlDomain))
                        .ToString(),
                        RedirectSummaryType = string.Format(
                            "parsed and valid old urls has domain '{0}'",
                            oldUrlDomain)
                    });
            }

            // get new url domains
            var newUrlDomains =
                redirectProcessingResult.ProcessedRedirects
                .Where(
                    pr => pr.ParsedRedirect.IsValid)
                .Select(
                    pr => pr.ParsedRedirect.NewUrl.Parsed.Host.ToLower())
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

            // domains in parsed and valid old urls
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = newUrlDomains.Count.ToString(),
                    RedirectSummaryType = "domains in parsed and valid new urls"
                });

            // parsed and valid new urls has domain
            foreach (var newUrlDomain in newUrlDomains)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = redirectProcessingResult
                        .ParsedRedirects
                        .Count(pr => pr.IsValid && pr.NewUrl.Parsed.Host.Equals(newUrlDomain))
                        .ToString(),
                        RedirectSummaryType = string.Format(
                            "parsed and valid new urls has domain '{0}'",
                            newUrlDomain)
                    });
            }

            // processed redirects summary
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ProcessedRedirects
                    .Count()
                    .ToString(),
                    RedirectSummaryType = "processed redirects"
                });

            // get result types from processed redirects
            var resultTypes =
                redirectProcessingResult.ProcessedRedirects
                .SelectMany(
                    pr => pr.Results.Select(r => r.Type))
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

            // result types in processed redirects
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = resultTypes.Count.ToString(),
                    RedirectSummaryType = "result types in processed redirects"
                });

            // processed redirects has result types
            foreach (var resultType in resultTypes)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = redirectProcessingResult
                        .ProcessedRedirects
                        .Sum(
                        pr => pr.Results.Count(r => r.Type.Equals(resultType))).ToString(),
                        RedirectSummaryType = string.Format(
                            "processed redirects has result type '{0}'",
                            resultType)
                    });
            }

            // url response results
            var urlResponseResults = redirectProcessingResult
                .ProcessedRedirects
                .SelectMany(
                        pr => pr.Results.OfType<UrlResponseResult>().Where(
                            r => r.Type.Equals(ResultTypes.UrlResponse)))
                            .ToList();

            // url response result status codes
            var statusCodes = urlResponseResults
                .Select(r => r.StatusCode)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            // url response results summary
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = statusCodes
                    .Count()
                    .ToString(),
                    RedirectSummaryType = "status codes in url response results"
                });

            // url response results has status code
            foreach (var statusCode in statusCodes)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = urlResponseResults
                        .Count(r => r.StatusCode == statusCode).ToString(),
                        RedirectSummaryType = string.Format(
                            "url response results has status code '{0}'",
                            statusCode)
                    });
            }

            // get valid processed redirects counts
            var validProcessedRedirectsCount = 0;
            var validProcessedRedirectsIncludngNotMatchingCount = 0;
            foreach (var processedRedirect in redirectProcessingResult.ProcessedRedirects)
            {
                if (_processedRedirectValidator.IsValid(processedRedirect, false))
                {
                    validProcessedRedirectsCount++;
                }
                if (_processedRedirectValidator.IsValid(processedRedirect, true))
                {
                    validProcessedRedirectsIncludngNotMatchingCount++;
                }
            }

            // valid processed redirects counts
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = string.Format(
                        "valid processed redirects, which have url response result with status code 200 and doesn't have result types '{0}'",
                        string.Join(",", _processedRedirectValidator.InvalidResultTypes))
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount =
                    validProcessedRedirectsCount.ToString(),
                    RedirectSummaryType = "redirects matching new url"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount =
                    validProcessedRedirectsIncludngNotMatchingCount.ToString(),
                    RedirectSummaryType = "redirects both matching and not matching new url"
                });

            // start, end and elapsed time
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord());
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = "processing time"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = string.Format(
                        "processing started at '{0}'",
                        redirectProcessingResult.StartTime.ToString("o"))
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = string.Format(
                        "processing ended at '{0}'",
                        redirectProcessingResult.EndTime.ToString("o"))
                });
            var elapsedTime =
                redirectProcessingResult.EndTime -
                redirectProcessingResult.StartTime;
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = string.Format(
                        "processing elapsed for '{0}'",
                        elapsedTime)
                });
        }

        public override IEnumerable<RedirectSummaryReportRecord> GetRecords()
        {
            return _redirectSummaryReportRecords;
        }
    }
}