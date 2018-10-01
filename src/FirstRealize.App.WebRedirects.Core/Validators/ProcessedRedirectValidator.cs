using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Validators
{
    public class ProcessedRedirectValidator : IProcessedRedirectValidator
    {
        private readonly IConfiguration _configuration;
        private readonly IUrlHelper _urlHelper;
        private readonly IList<string> _invalidResultTypes;

        public ProcessedRedirectValidator(
            IConfiguration configuration,
            IUrlHelper urlHelper)
        {
            _configuration = configuration;
            _urlHelper = urlHelper;
            _invalidResultTypes = new List<string>
            {
                ResultTypes.CyclicRedirect,
                ResultTypes.ExcludedRedirect,
                ResultTypes.IdenticalResult,
                ResultTypes.InvalidResult,
                ResultTypes.TooManyRedirects,
                ResultTypes.UnknownErrorResult
            };

            switch (_configuration.DuplicateOldUrlStrategy)
            {
                case DuplicateUrlStrategy.KeepFirst:
                    _invalidResultTypes.Add(ResultTypes.DuplicateOfFirst);
                    break;
                case DuplicateUrlStrategy.KeepLast:
                    _invalidResultTypes.Add(ResultTypes.DuplicateOfLast);
                    break;
            }
        }

        public IList<string> InvalidResultTypes
        {
            get
            {
                return _invalidResultTypes;
            }
        }

        public bool IsValid(
            IProcessedRedirect processedRedirect,
            bool includeNotMatchingNewUrl)
        {
            if (processedRedirect.Results.Any(
                r => _invalidResultTypes.Contains(r.Type, StringComparer.OrdinalIgnoreCase)))
            {
                return false;
            }
            else
            {
                var urlResponseResult = processedRedirect
                .Results
                .OfType<UrlResponseResult>()
                .FirstOrDefault(r => r.Type.Equals(ResultTypes.UrlResponse));
                if (urlResponseResult == null)
                {
                    return false;
                }

                return includeNotMatchingNewUrl
                    ? urlResponseResult.StatusCode == 200
                    : _urlHelper.AreIdentical(
                    processedRedirect.ParsedRedirect.NewUrl.Formatted,
                    urlResponseResult.Url) &&
                    urlResponseResult.StatusCode == 200;
            }
        }
    }
}