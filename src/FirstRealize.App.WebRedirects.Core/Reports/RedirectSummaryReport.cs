using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class RedirectSummaryReport : ReportBase<RedirectSummaryReportRecord>
    {
        private readonly IList<RedirectSummaryReportRecord> _redirectSummaryReportRecords;

        public RedirectSummaryReport()
        {
            _redirectSummaryReportRecords = 
                new List<RedirectSummaryReportRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            // processors
            var processors = redirectProcessingResult
                .Processors
                .Select(p => p.Name)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            // processors used
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = processors.Count,
                    RedirectSummaryType = "processors used"
                });

            // results from processor
            foreach(var processor in processors)
            {
                _redirectSummaryReportRecords.Add(
                    new RedirectSummaryReportRecord
                    {
                        RedirectSummaryCount = redirectProcessingResult
                        .ParsedRedirects
                        .Count(),
                        RedirectSummaryType = string.Format(
                            "result(s) from processor '{0}'",
                            processor)
                    });
            }

            // parsed redirects
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = "---"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ParsedRedirects
                    .Count(),
                    RedirectSummaryType = "parsed redirects"
                });

            // are valid redirects
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ProcessedRedirects
                    .Count(
                        pr => pr.ParsedRedirect.IsValid),
                    RedirectSummaryType = "valid redirects"
                });

            // are identical redirects
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ProcessedRedirects
                    .Count(
                        pr => pr.ParsedRedirect.IsIdentical),
                    RedirectSummaryType = "identical redirects"
                });

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
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = "---"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = oldUrlDomains.Count,
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
                        .Count(pr => pr.IsValid && pr.OldUrl.Parsed.Host.Equals(oldUrlDomain)),
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
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = "---"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = newUrlDomains.Count,
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
                        .Count(pr => pr.IsValid && pr.NewUrl.Parsed.Host.Equals(newUrlDomain)),
                        RedirectSummaryType = string.Format(
                            "parsed and valid new urls has domain '{0}'",
                            newUrlDomain)
                    });
            }

            // processed redirects summary
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryType = "---"
                });
            _redirectSummaryReportRecords.Add(
                new RedirectSummaryReportRecord
                {
                    RedirectSummaryCount = redirectProcessingResult
                    .ProcessedRedirects
                    .Count(),
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
                    RedirectSummaryCount = resultTypes.Count,
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
                        pr => pr.Results.Count(r => r.Type.Equals(resultType))),
                        RedirectSummaryType = string.Format(
                            "processed redirects has result type '{0}'",
                            resultType)
                    });
            }
        }

        public override IEnumerable<RedirectSummaryReportRecord> GetRecords()
        {
            return _redirectSummaryReportRecords;
        }
    }
}