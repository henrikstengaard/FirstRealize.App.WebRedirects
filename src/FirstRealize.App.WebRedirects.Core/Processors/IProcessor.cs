using FirstRealize.App.WebRedirects.Core.Models;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessor
    {
        void Process(IProcessedRedirect processedRedirect);
        IEnumerable<IResult> Results { get; }
    }
}