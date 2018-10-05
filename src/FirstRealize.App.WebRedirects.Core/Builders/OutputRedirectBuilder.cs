using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Validators;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Builders
{
    public class OutputRedirectBuilder : IOutputRedirectBuilder
    {
        private readonly IProcessedRedirectValidator _processedRedirectValidator;

        public OutputRedirectBuilder(
            IProcessedRedirectValidator processedRedirectValidator)
        {
            _processedRedirectValidator = processedRedirectValidator;
        }

        public OutputRedirect Build(
            IProcessedRedirect processedRedirect)
        {
            var urlResponseResult = processedRedirect
            .Results
            .OfType<UrlResponseResult>()
            .FirstOrDefault(r => r.Type.Equals(ResultTypes.UrlResponse));

            var newUrl = urlResponseResult != null && !string.IsNullOrWhiteSpace(urlResponseResult.Url)
                ? urlResponseResult.Url
                : processedRedirect.ParsedRedirect.NewUrl.Formatted;

            return new OutputRedirect
            {
                OldUrl = processedRedirect.ParsedRedirect.OldUrl.Formatted,
                NewUrl = newUrl,
                ValidMatchingOriginalNewUrl = _processedRedirectValidator.IsValid(
                processedRedirect,
                false),
                ValidNotMatchingOriginalNewUrl = _processedRedirectValidator.IsValid(
                processedRedirect,
                true)
            };
        }
    }
}