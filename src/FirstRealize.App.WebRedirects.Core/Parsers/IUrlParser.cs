using System;

namespace FirstRealize.App.WebRedirects.Core.Parsers
{
    public interface IUrlParser
    {
        Uri ParseUrl(
            string url,
            Uri host = null,
            bool stripFragment = false);
    }
}