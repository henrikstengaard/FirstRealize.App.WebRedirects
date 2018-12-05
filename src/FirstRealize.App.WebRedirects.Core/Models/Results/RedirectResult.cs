using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models.Results
{
    public class RedirectResult : Result
    {
        public int RedirectCount { get; set; }
		public IList<string> UrlsVisited { get; set; }
		public IList<IParsedRedirect> RedirectsVisited { get; set; }

		public RedirectResult()
		{
			UrlsVisited = new List<string>();
			RedirectsVisited = new List<IParsedRedirect>();
		}
	}
}