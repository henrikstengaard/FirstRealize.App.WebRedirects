using FirstRealize.App.WebRedirects.Core.Exporters;
using FirstRealize.App.WebRedirects.Core.Processors;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectEngine
    {
        event EventHandler<RedirectProcessedEventArgs> RedirectProcessed;
        IList<IProcessor> Processors { get; }
        IList<IExporter> Exporters { get; }
        IRedirectProcessingResult Run();
    }
}