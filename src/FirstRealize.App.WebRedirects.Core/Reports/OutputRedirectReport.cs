using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Builders;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class OutputRedirectReport : ReportBase<OutputRedirectRecord>
    {
        private readonly IOutputRedirectBuilder _outputRedirectBuilder;
        private readonly bool _includeNotMatchingNewUrl;
        private readonly IList<OutputRedirectRecord> _records;

        public OutputRedirectReport(
            IOutputRedirectBuilder outputRedirectBuilder,
            bool includeNotMatchingNewUrl)
        {
            _outputRedirectBuilder = outputRedirectBuilder;
            _includeNotMatchingNewUrl = includeNotMatchingNewUrl;
            _records =
                new List<OutputRedirectRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach (var processedRedirect in redirectProcessingResult.ProcessedRedirects.ToList())
            {
                var outputRedirect = _outputRedirectBuilder
                    .Build(processedRedirect);

                if (!outputRedirect.ValidMatchingOriginalNewUrl &&
                    !outputRedirect.ValidNotMatchingOriginalNewUrl)
                {
                    continue;
                }

                if (!outputRedirect.ValidMatchingOriginalNewUrl &&
                    outputRedirect.ValidNotMatchingOriginalNewUrl && 
                    !_includeNotMatchingNewUrl)
                {
                    continue;
                }

                var record = new OutputRedirectRecord
                {
                    OldUrl = outputRedirect.OldUrl,
                    NewUrl = outputRedirect.NewUrl
                };

                if (processedRedirect.ParsedRedirect != null)
                {
                    if (processedRedirect.ParsedRedirect.OldUrl != null)
                    {
                        record.OriginalOldUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.OldUrl);
                        record.OldUrlHasHost =
                        record.OriginalOldUrlHasHost =
                            processedRedirect.ParsedRedirect.OldUrl.Parsed.OriginalUrlHasHost;
                        record.ParsedOldUrl =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.OldUrl);

                    }

                    if (processedRedirect.ParsedRedirect.NewUrl != null)
                    {
                        record.OriginalNewUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.NewUrl);
                        record.NewUrlHasHost =
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