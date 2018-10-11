using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Exporters
{
    public class Rewrite
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool OldUrlHasRootPath { get; set; }
        public bool OldUrlHasHost { get; set; }
        public bool NewUrlHasHost { get; set; }
        public IParsedUrl OldUrl { get; set; }
        public IParsedUrl NewUrl { get; set; }
        public IList<Rewrite> RelatedRewrites { get; set; }

        public Rewrite()
        {
            RelatedRewrites = new List<Rewrite>();
        }
    }
}