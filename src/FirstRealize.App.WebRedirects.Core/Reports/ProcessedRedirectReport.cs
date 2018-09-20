using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class ProcessedRedirectReport : ReportBase<ProcessedRedirectRecord>
    {
        private readonly IList<ProcessedRedirectRecord> _processedRedirectRecords;

        public ProcessedRedirectReport()
        {
            _processedRedirectRecords =
                new List<ProcessedRedirectRecord>();
        }

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            var processedRedirects = redirectProcessingResult
                .ProcessedRedirects
                .ToList();

            foreach(var processedRedirect in processedRedirects)
            {
                var processedRedirectRecord =
                    new ProcessedRedirectRecord
                    {
                        Valid = processedRedirect.ParsedRedirect.IsValid,
                        Identical = processedRedirect.ParsedRedirect.IsIdentical
                    };

                if (processedRedirect.ParsedRedirect != null)
                {
                    if (processedRedirect.ParsedRedirect.OldUrl != null)
                    {
                        processedRedirectRecord.OldUrlRaw =
                            processedRedirect.ParsedRedirect.OldUrl != null
                            ? processedRedirect.ParsedRedirect.OldUrl.Raw ?? string.Empty
                            : string.Empty;
                        processedRedirectRecord.OldUrlParsed =
                            processedRedirect.ParsedRedirect.OldUrl.Parsed != null
                            ? processedRedirect.ParsedRedirect.OldUrl.Parsed.AbsoluteUri
                            : string.Empty;
                    }

                    if (processedRedirect.ParsedRedirect.NewUrl != null)
                    {
                        processedRedirectRecord.NewUrlRaw =
                            processedRedirect.ParsedRedirect.NewUrl != null
                            ? processedRedirect.ParsedRedirect.NewUrl.Raw ?? string.Empty
                            : string.Empty;
                        processedRedirectRecord.NewUrlParsed =
                            processedRedirect.ParsedRedirect.NewUrl.Parsed != null
                            ? processedRedirect.ParsedRedirect.NewUrl.Parsed.AbsoluteUri
                            : string.Empty;
                    }
                }

                var resultTypes = processedRedirect.Results
                    .Select(x => x.Type)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                processedRedirectRecord.ResultCount = 
                    resultTypes.Count;
                processedRedirectRecord.ResultTypes = 
                    string.Join(",", resultTypes);

                var excludedResult = processedRedirect.Results
                    .FirstOrDefault(r => r.Type.Equals(ResultTypes.ExcludedRedirect));

                if (excludedResult != null)
                {
                    processedRedirectRecord.ExcludedRedirect = true;
                    processedRedirectRecord.ExcludedRedirectMessage = excludedResult.Message;
                    processedRedirectRecord.ExcludedRedirectUrl =
                        excludedResult.Url != null &&
                        excludedResult.Url.Parsed != null
                        ? excludedResult.Url.Parsed.AbsoluteUri
                        : string.Empty;
                }

                var duplicateOfFirstResult = processedRedirect.Results
                    .FirstOrDefault(r => r.Type.Equals(ResultTypes.DuplicateOfFirst));

                if (duplicateOfFirstResult != null)
                {
                    processedRedirectRecord.DuplicateOfFirst = true;
                    processedRedirectRecord.DuplicateOfFirstMessage = duplicateOfFirstResult.Message;
                    processedRedirectRecord.DuplicateOfFirstUrl =
                        duplicateOfFirstResult.Url != null &&
                        duplicateOfFirstResult.Url.Parsed != null
                        ? duplicateOfFirstResult.Url.Parsed.AbsoluteUri
                        : string.Empty;
                }

                var duplicateOfLastResult = processedRedirect.Results
                    .FirstOrDefault(r => r.Type.Equals(ResultTypes.DuplicateOfLast));

                if (duplicateOfLastResult != null)
                {
                    processedRedirectRecord.DuplicateOfLast = true;
                    processedRedirectRecord.DuplicateOfLastMessage = duplicateOfLastResult.Message;
                    processedRedirectRecord.DuplicateOfLastUrl =
                        duplicateOfLastResult.Url != null &&
                        duplicateOfLastResult.Url.Parsed != null
                        ? duplicateOfLastResult.Url.Parsed.AbsoluteUri
                        : string.Empty;
                }

                _processedRedirectRecords.Add(
                    processedRedirectRecord);
            }
        }

        public override IEnumerable<ProcessedRedirectRecord> GetRecords()
        {
            return _processedRedirectRecords;
        }
    }
}