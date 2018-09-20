using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IUrlParser
    {
        IUrl ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false);
    }
}