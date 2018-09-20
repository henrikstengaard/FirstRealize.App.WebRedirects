using FirstRealize.App.WebRedirects.Core.Processors;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectEngine
    {
        IList<IProcessor> Processors { get; }
        IRedirectProcessingResult Run();
    }
}