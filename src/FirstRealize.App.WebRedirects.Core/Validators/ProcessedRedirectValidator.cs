using FirstRealize.App.WebRedirects.Core.Helpers;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Validators
{
    public class ProcessedRedirectValidator : IProcessedRedirectValidator
    {
        private readonly IUrlHelper _urlHelper;

        public ProcessedRedirectValidator(
            IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public bool IsValid(
            IProcessedRedirect processedRedirect,
            bool includeNotMatchingNewUrl)
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
                if (urlResponseResult == null)
                {
                    return false;
                }

                return includeNotMatchingNewUrl
                    ? urlResponseResult.StatusCode == 200
                    : _urlHelper.AreIdentical(
                    processedRedirect.ParsedRedirect.NewUrl,
                    urlResponseResult.Url) &&
                    urlResponseResult.StatusCode == 200;
            }
        }
    }
}