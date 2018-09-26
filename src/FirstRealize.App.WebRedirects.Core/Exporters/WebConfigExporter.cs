using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System.Collections.Generic;
using System.IO;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class WebConfigExporter : IExporter
    {
        public string Name => "WebConfig";

        public void Export(
            IEnumerable<IRedirect> redirects,
            string outputDir)
        {
            foreach (var redirect in redirects)
            {

            }

            var webConfigFile = Path.Combine(
                outputDir,
                "web.config");
        }
    }
}