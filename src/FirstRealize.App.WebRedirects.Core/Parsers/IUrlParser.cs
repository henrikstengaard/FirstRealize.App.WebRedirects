using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Urls;
using System;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
	public interface IUrlParser
    {
		IParsedUrl Parse(
			string url,
			IParsedUrl defaultUrl = null,
			bool stripFragment = false);
		IUrl ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false);
    }
}