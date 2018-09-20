using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessorPreload
    {
        void PreloadParsedRedirects(
            IEnumerable<IParsedRedirect> parsedRedirects);
    }
}