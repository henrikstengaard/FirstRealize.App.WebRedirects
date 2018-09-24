using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Validators
{
    public interface IProcessedRedirectValidator
    {
        bool IsValid(
            IProcessedRedirect processedRedirect);
    }
}