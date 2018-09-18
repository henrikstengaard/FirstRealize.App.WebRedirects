using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessorPreload
    {
        void PreloadRedirects(IEnumerable<Redirect> redirects);
    }
}