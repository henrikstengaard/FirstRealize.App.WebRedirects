using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Validators;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class FilteredRedirectReport : ReportBase<FilteredRedirectRecord>
    {
        private readonly IProcessedRedirectValidator _processedRedirectValidator;
        private readonly bool _includeNotMatchingNewUrl;
        private readonly IList<FilteredRedirectRecord> _records;


        public FilteredRedirectReport(
            IProcessedRedirectValidator processedRedirectValidator,
            bool includeNotMatchingNewUrl)
        {
            _processedRedirectValidator = processedRedirectValidator;
            _includeNotMatchingNewUrl = includeNotMatchingNewUrl;
            _records =
                new List<FilteredRedirectRecord>();
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
                    : processedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri;

                var record = new FilteredRedirectRecord
                {
                    OldUrlResult = processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri,
                    NewUrlResult = newUrl
                };

                if (processedRedirect.ParsedRedirect != null)
                {
                    if (processedRedirect.ParsedRedirect.OldUrl != null)
                    {
                        record.OldUrlRaw =
                            FormatRawUrl(processedRedirect.ParsedRedirect.OldUrl);
                        record.OldUrlHasHost =
                            processedRedirect.ParsedRedirect.OldUrl.HasHost;
                        record.OldUrlParsed =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.OldUrl);
                    }

                    if (processedRedirect.ParsedRedirect.NewUrl != null)
                    {
                        record.NewUrlRaw =
                            FormatRawUrl(processedRedirect.ParsedRedirect.NewUrl);
                        record.NewUrlHasHost =
                            processedRedirect.ParsedRedirect.NewUrl.HasHost;
                        record.NewUrlParsed =
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
                ? url.Parsed.AbsoluteUri
                : string.Empty;
        }

        public override IEnumerable<FilteredRedirectRecord> GetRecords()
        {
            return _records;
        }
    }
}