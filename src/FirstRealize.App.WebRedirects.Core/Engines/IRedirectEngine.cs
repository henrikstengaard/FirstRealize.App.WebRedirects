using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectEngine
    {
        IEnumerable<Redirect> Redirects { get; }
        IEnumerable<IProcessedRedirect> ProcessedRedirects { get; }
        IEnumerable<IResult> Results { get; }
        void Run();
    }
}