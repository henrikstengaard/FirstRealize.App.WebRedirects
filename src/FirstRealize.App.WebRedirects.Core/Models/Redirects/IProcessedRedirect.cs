using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public interface IProcessedRedirect
    {
        IParsedRedirect ParsedRedirect { get; }
        IList<IResult> Results { get; }
    }
}