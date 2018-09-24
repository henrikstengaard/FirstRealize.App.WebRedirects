using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class FilteredRedirectReport : ReportBase<FilteredRedirectRecord>
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IList<FilteredRedirectRecord> _records;

        public FilteredRedirectReport(
            IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
            _records =
                new List<FilteredRedirectRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach(var processedRedirect in redirectProcessingResult.ProcessedRedirects.ToList())
            {
                if (!IsValid(processedRedirect))
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

        private bool IsValid(
            IProcessedRedirect processedRedirect)
        {
            if (processedRedirect.Results.Any(
                r => r.Type.Equals(ResultTypes.UnknownErrorResult)))
            {
                return false;
            }
            else if (processedRedirect.Results.Any(
                r => r.Type.Equals(ResultTypes.ExcludedRedirect)))
            {
                return false;
            }
            else if (processedRedirect.Results.Any(
                r => r.Type.Equals(ResultTypes.InvalidResult)))
            {
                return false;
            }
            else if (processedRedirect.Results.Any(
                r => r.Type.Equals(ResultTypes.CyclicRedirect)))
            {
                return false;
            }
            else if (processedRedirect.Results.Any(
                r => r.Type.Equals(ResultTypes.TooManyRedirects)))
            {
                return false;
            }
            else
            {
                var urlResponseResult = processedRedirect
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.UrlResponse));
                if (urlResponseResult != null && _urlHelper.AreIdentical(
                    processedRedirect.ParsedRedirect.NewUrl,
                    urlResponseResult.Url) &&
                    urlResponseResult.StatusCode == 200)
                {
                    return true;
                }
            }

            return false;
        }

        public override IEnumerable<FilteredRedirectRecord> GetRecords()
        {
            return _records;
        }
    }
}