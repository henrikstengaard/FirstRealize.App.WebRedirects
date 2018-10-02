using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Validators;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class OutputRedirectReport : ReportBase<OutputRedirectRecord>
    {
        private readonly IProcessedRedirectValidator _processedRedirectValidator;
        private readonly bool _includeNotMatchingNewUrl;
        private readonly IList<OutputRedirectRecord> _records;

        public OutputRedirectReport(
            IProcessedRedirectValidator processedRedirectValidator,
            bool includeNotMatchingNewUrl)
        {
            _processedRedirectValidator = processedRedirectValidator;
            _includeNotMatchingNewUrl = includeNotMatchingNewUrl;
            _records =
                new List<OutputRedirectRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach (var processedRedirect in redirectProcessingResult.ProcessedRedirects.ToList())
            {
                if (!_processedRedirectValidator.IsValid(
                    processedRedirect,
                    _includeNotMatchingNewUrl))
                {
                    continue;
                }

                var urlResponseResult = processedRedirect
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.UrlResponse));

                var newUrl = urlResponseResult != null && !string.IsNullOrWhiteSpace(urlResponseResult.Url)
                    ? urlResponseResult.Url
                    : processedRedirect.ParsedRedirect.NewUrl.Formatted;

                var record = new OutputRedirectRecord
                {
                    OldUrl = processedRedirect.ParsedRedirect.OldUrl.Formatted,
                    NewUrl = newUrl
                };

                if (processedRedirect.ParsedRedirect != null)
                {
                    if (processedRedirect.ParsedRedirect.OldUrl != null)
                    {
                        record.OriginalOldUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.OldUrl);
                        record.OriginalOldUrlHasHost =
                            processedRedirect.ParsedRedirect.OldUrl.Parsed.OriginalUrlHasHost;
                        record.ParsedOldUrl =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.OldUrl);
                    }

                    if (processedRedirect.ParsedRedirect.NewUrl != null)
                    {
                        record.OriginalNewUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.NewUrl);
                        record.OriginalNewUrlHasHost =
                            processedRedirect.ParsedRedirect.NewUrl.Parsed.OriginalUrlHasHost;
                        record.ParsedNewUrl =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.NewUrl);
                    }
                }

                _records.Add(record);
            }
        }

        private string FormatRawUrl(IUrl url)
        {
            return url != null && url.Raw != null
                ? url.Raw ?? string.Empty
                : string.Empty;
        }

        private string FormatParsedUrl(IUrl url)
        {
            return url != null && url.Parsed != null
                ? url.Formatted
                : string.Empty;
        }

        public override IEnumerable<OutputRedirectRecord> GetRecords()
        {
            return _records;
        }
    }
}