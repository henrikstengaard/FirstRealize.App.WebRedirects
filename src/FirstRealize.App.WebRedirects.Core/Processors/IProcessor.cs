using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessor
    {
        void Process(IProcessedRedirect processedRedirect);
        IEnumerable<Result> Results { get; }
    }
}