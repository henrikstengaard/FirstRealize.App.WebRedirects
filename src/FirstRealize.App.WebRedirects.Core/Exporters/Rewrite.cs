using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class Rewrite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IRedirect Redirect { get; set; }
        public IParsedUrl OldUrl { get; set; }
        public IParsedUrl NewUrl { get; set; }
        public string MatchUrl { get; set; }
        public string RedirectUrl { get; set; }
        public IDictionary<string, string> RewriteMap { get; set; }

        public Rewrite()
        {
            RewriteMap = new Dictionary<string, string>();
        }
    }
}