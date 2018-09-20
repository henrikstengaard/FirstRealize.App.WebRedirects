using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FirstRealize.App.WebRedirects.Core.Processors;

namespace FirstRealize.App.WebRedirects.Core.Resolvers
{
    public class ProcessorResolver : IProcessorResolver
    {
        public IEnumerable<IProcessor> ResolveProcessors()
        {
            // get types that implement iprocessor interface
            var processorType = typeof(IProcessor);
            var processorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.GetTypeInfo().IsClass && processorType.IsAssignableFrom(p));

            // create instance of each processor type
            return processorTypes.Select(t => (IProcessor)Activator.CreateInstance(t));
        }
    }
}