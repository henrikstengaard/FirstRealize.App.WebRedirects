using FirstRealize.App.WebRedirects.Core.Processors;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Resolvers
{
    public interface IProcessorResolver
    {
        IEnumerable<IProcessor> ResolveProcessors();
    }
}