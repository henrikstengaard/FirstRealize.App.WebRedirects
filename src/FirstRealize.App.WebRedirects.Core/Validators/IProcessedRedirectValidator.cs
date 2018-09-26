using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Validators
{
    public interface IProcessedRedirectValidator
    {
        IList<string> InvalidResultTypes { get; }
        bool IsValid(
            IProcessedRedirect processedRedirect,
            bool includeNotMatchingNewUrl);
    }
}