using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IParsedRedirect : IComparable
    {
        IUrl OldUrl { get; }
        IUrl NewUrl { get; }
		RedirectType RedirectType { get; }
		bool IsValid { get; }
    }
}