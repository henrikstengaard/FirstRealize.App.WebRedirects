using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IUrl
    {
        string Raw { get; }
        Uri Parsed { get; }
        bool IsValid { get; }
        bool HasHost { get; }
    }
}