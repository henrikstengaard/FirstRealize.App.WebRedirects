using FirstRealize.App.WebRedirects.Core.Models;
using System;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IUrlParser
    {
        Url ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false);
    }
}