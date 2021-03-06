﻿using System.Collections.Generic;
using System.Linq;
using FirstRealize.App.WebRedirects.Core.Builders;
using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using FirstRealize.App.WebRedirects.Core.Models.Results;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class ProcessedRedirectReport : ReportBase<ProcessedRedirectRecord>
    {
        private readonly IOutputRedirectBuilder _outputRedirectBuilder;
        private readonly IList<ProcessedRedirectRecord> _processedRedirectRecords;

        public ProcessedRedirectReport(
            IOutputRedirectBuilder outputRedirectBuilder)
        {
            _outputRedirectBuilder = outputRedirectBuilder;
            _processedRedirectRecords =
                new List<ProcessedRedirectRecord>();
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

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            var processedRedirects = redirectProcessingResult
                .ProcessedRedirects
                .ToList();

            foreach (var processedRedirect in processedRedirects)
            {
                var processedRedirectRecord =
                    new ProcessedRedirectRecord();

                if (processedRedirect.ParsedRedirect != null)
                {
                    if (processedRedirect.ParsedRedirect.OldUrl != null)
                    {
                        processedRedirectRecord.OriginalOldUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.OldUrl);
                        processedRedirectRecord.OriginalOldUrlHasHost =
                            processedRedirect.ParsedRedirect.OldUrl.Parsed.OriginalUrlHasHost;
                        processedRedirectRecord.ParsedOldUrl =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.OldUrl);
                    }

                    if (processedRedirect.ParsedRedirect.NewUrl != null)
                    {
                        processedRedirectRecord.OriginalNewUrl =
                            FormatRawUrl(processedRedirect.ParsedRedirect.NewUrl);
                        processedRedirectRecord.OriginalNewUrlHasHost =
                            processedRedirect.ParsedRedirect.NewUrl.Parsed.OriginalUrlHasHost;
                        processedRedirectRecord.ParsedNewUrl =
                            FormatParsedUrl(processedRedirect.ParsedRedirect.NewUrl);
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

                // output redirect
                AddOutputRedirectUrls(
                    processedRedirect,
                    processedRedirectRecord);

                // invalid redirect result
                AddInvalidRedirect(
                    processedRedirect,
                    processedRedirectRecord);

                // identical redirect result
                AddIdenticalRedirect(
                    processedRedirect,
                    processedRedirectRecord);

                // excluded redirect result
                AddExcludedRedirectResult(
                    processedRedirect,
                    processedRedirectRecord);

                // duplicate of first result
                AddDuplicateOfFirstResult(
                    processedRedirect,
                    processedRedirectRecord);

                // duplicate of last result
                AddDuplicateOfLastResult(
                    processedRedirect,
                    processedRedirectRecord);

                // url response result
                AddUrlResponseResult(
                    processedRedirect,
                    processedRedirectRecord);

                // optimized redirect result
                AddOptimizedRedirectResult(
                    processedRedirect,
                    processedRedirectRecord);

                // cyclic redirect result
                AddCyclicRedirectResult(
                    processedRedirect,
                    processedRedirectRecord);

                // too many redirects result
                AddTooManyRedirectsResult(
                    processedRedirect,
                    processedRedirectRecord);

                // unknown error result
                AddUnknownError(
                    processedRedirect,
                    processedRedirectRecord);

                _processedRedirectRecords.Add(
                    processedRedirectRecord);
            }
        }

        private void AddOutputRedirectUrls(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var outputRedirect = _outputRedirectBuilder.Build(
                processedRedirect);

            processedRedirectRecord.OutputRedirectOldUrl = 
                outputRedirect.OldUrl;
            processedRedirectRecord.OutputRedirectNewUrl = 
                outputRedirect.NewUrl;
            processedRedirectRecord.OutputRedirectValidMatchingOriginalNewUrl = 
                outputRedirect.ValidMatchingOriginalNewUrl;
            processedRedirectRecord.OutputRedirectValidNotMatchingOriginalNewUrl =
                outputRedirect.ValidNotMatchingOriginalNewUrl;
        }

        private void AddInvalidRedirect(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var invalidRedirectResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.InvalidResult));

            if (invalidRedirectResult == null)
            {
                return;
            }

            processedRedirectRecord.InvalidRedirect = true;
            processedRedirectRecord.InvalidRedirectMessage =
                invalidRedirectResult.Message;
        }

        private void AddIdenticalRedirect(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var identicalRedirectResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.IdenticalResult));

            if (identicalRedirectResult == null)
            {
                return;
            }

            processedRedirectRecord.IdenticalRedirect = true;
            processedRedirectRecord.IdenticalRedirectMessage =
                identicalRedirectResult.Message;
        }

        private void AddExcludedRedirectResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var excludedRedirectResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.ExcludedRedirect));

            if (excludedRedirectResult == null)
            {
                return;
            }

            processedRedirectRecord.ExcludedRedirect = true;
            processedRedirectRecord.ExcludedRedirectMessage = excludedRedirectResult.Message;
            processedRedirectRecord.ExcludedRedirectUrl =
                excludedRedirectResult.Url;
        }

        private void AddDuplicateOfFirstResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var duplicateOfFirstResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.DuplicateOfFirst));

            if (duplicateOfFirstResult == null)
            {
                return;
            }

            processedRedirectRecord.DuplicateOfFirst = true;
            processedRedirectRecord.DuplicateOfFirstMessage =
                duplicateOfFirstResult.Message;
            processedRedirectRecord.DuplicateOfFirstUrl =
                duplicateOfFirstResult.Url;
        }

        private void AddDuplicateOfLastResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var duplicateOfLastResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.DuplicateOfLast));

            if (duplicateOfLastResult == null)
            {
                return;
            }

            processedRedirectRecord.DuplicateOfLast = true;
            processedRedirectRecord.DuplicateOfLastMessage =
                duplicateOfLastResult.Message;
            processedRedirectRecord.DuplicateOfLastUrl =
                duplicateOfLastResult.Url;
        }

        private void AddUrlResponseResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var urlResponseResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.UrlResponse)) as UrlResponseResult;

            if (urlResponseResult == null)
            {
                return;
            }

            processedRedirectRecord.UrlResponse = true;
            processedRedirectRecord.UrlResponseMessage =
                urlResponseResult.Message;
            processedRedirectRecord.UrlResponseUrl =
                urlResponseResult.Url;
            processedRedirectRecord.UrlResponseStatusCode =
                urlResponseResult.StatusCode;
            processedRedirectRecord.UrlResponseLocation =
                urlResponseResult.Location;
        }

        private void AddOptimizedRedirectResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var optimizedRedirectResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.OptimizedRedirect)) as RedirectResult;

            if (optimizedRedirectResult == null)
            {
                return;
            }

            processedRedirectRecord.OptimizedRedirect = true;
            processedRedirectRecord.OptimizedRedirectMessage =
                optimizedRedirectResult.Message;
            processedRedirectRecord.OptimizedRedirectUrl =
                optimizedRedirectResult.Url;
            processedRedirectRecord.OptimizedRedirectCount =
                optimizedRedirectResult.RedirectCount;
			processedRedirectRecord.OptimizedRedirectVisitedUrls =
				string.Join(
					",",
					optimizedRedirectResult.UrlsVisited);
			processedRedirectRecord.OptimizedRedirectVisitedRedirects = 
				string.Join(
					",",
					optimizedRedirectResult.RedirectsVisited.Select(
						x => string.Format(
							"{0} -> {1}",
							x.OldUrl.Formatted,
							x.NewUrl.Formatted)));
        }

        private void AddCyclicRedirectResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var cyclicRedirectResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.CyclicRedirect)) as RedirectResult;

            if (cyclicRedirectResult == null)
            {
                return;
            }

            processedRedirectRecord.CyclicRedirect = true;
            processedRedirectRecord.CyclicRedirectMessage =
                cyclicRedirectResult.Message;
            processedRedirectRecord.CyclicRedirectUrl =
                cyclicRedirectResult.Url;
            processedRedirectRecord.CyclicRedirectCount =
                cyclicRedirectResult.RedirectCount;
			processedRedirectRecord.CyclicRedirectVisitedUrls =
				string.Join(
					",",
					cyclicRedirectResult.UrlsVisited);
			processedRedirectRecord.CyclicRedirectVisitedRedirects =
				string.Join(
					",",
					cyclicRedirectResult.RedirectsVisited.Select(
						x => string.Format(
							"{0} -> {1}",
							x.OldUrl.Formatted,
							x.NewUrl.Formatted)));
		}

		private void AddTooManyRedirectsResult(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var tooManyRedirectsResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.TooManyRedirects)) as RedirectResult;

            if (tooManyRedirectsResult == null)
            {
                return;
            }

            processedRedirectRecord.TooManyRedirects = true;
            processedRedirectRecord.TooManyRedirectsMessage =
                tooManyRedirectsResult.Message;
            processedRedirectRecord.TooManyRedirectsUrl =
                tooManyRedirectsResult.Url;
            processedRedirectRecord.TooManyRedirectsCount =
                tooManyRedirectsResult.RedirectCount;
			processedRedirectRecord.TooManyRedirectsVisitedUrls = 
				string.Join(
					",",
					tooManyRedirectsResult.UrlsVisited);
			processedRedirectRecord.TooManyRedirectsVisitedRedirects =
				string.Join(
					",",
					tooManyRedirectsResult.RedirectsVisited.Select(
						x => string.Format(
							"{0} -> {1}",
							x.OldUrl.Formatted,
							x.NewUrl.Formatted)));
		}

		public override IEnumerable<ProcessedRedirectRecord> GetRecords()
        {
            return _processedRedirectRecords;
        }

        private void AddUnknownError(
            IProcessedRedirect processedRedirect,
            ProcessedRedirectRecord processedRedirectRecord)
        {
            var unknownErrorResult = processedRedirect.Results
                .FirstOrDefault(r => r.Type.Equals(
                    ResultTypes.UnknownErrorResult));

            if (unknownErrorResult == null)
            {
                return;
            }

            processedRedirectRecord.UnknownError = true;
            processedRedirectRecord.UnknownErrorMessage =
                unknownErrorResult.Message;
        }
    }
}