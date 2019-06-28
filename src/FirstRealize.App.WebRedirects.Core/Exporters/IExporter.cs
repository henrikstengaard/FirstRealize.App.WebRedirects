using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public interface IExporter
    {
        string Name { get; }
        string Build(
            IEnumerable<IRedirect> redirects);
        void Export(
            IEnumerable<IRedirect> redirects,
            string outputDir);
    }
}