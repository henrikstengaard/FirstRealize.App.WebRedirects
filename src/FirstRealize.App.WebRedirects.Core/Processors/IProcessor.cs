using FirstRealize.App.WebRedirects.Core.Models;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public interface IProcessor
    {
        void Process(IProcessedRedirect processedRedirect);
    }
}