using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;

namespace FirstRealize.App.WebRedirects.Core.Helpers
{
    public interface IUrlHelper
    {
        bool IsHttpsRedirect(
            IUrl oldUrl,
            IUrl newUrl);
        string FormatUrl(
            Uri parsedUrl);
    }
}
