using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
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
            foreach(var processedRedirect in redirectProcessingResult.ProcessedRedirects.ToList())
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

                var newUrl = urlResponseResult != null && urlResponseResult.Url != null
                    ? urlResponseResult.Url.Parsed.AbsoluteUri
                    : processedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri;

                _records.Add(
                    new FilteredRedirectRecord
                    {
                        OldUrl = processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri,
                        NewUrl = newUrl
                    });
            }
        }

        public override IEnumerable<FilteredRedirectRecord> GetRecords()
        {
            return _records;
        }
    }
}