using FirstRealize.App.WebRedirects.Core.Configuration;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessor
    {
        string Name { get; }
        IConfiguration Configuration { get; set; }
        void Process(IProcessedRedirect processedRedirect);
        IEnumerable<IResult> Results { get; }
    }
}