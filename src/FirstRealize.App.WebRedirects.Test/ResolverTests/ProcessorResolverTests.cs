using FirstRealize.App.WebRedirects.Core.Resolvers;
using NUnit.Framework;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Test.ResolverTests
{
    [TestFixture]
    public class ProcessorResolverTests
    {
        [Test]
        public void ResolveProcessors()
        {
            // create processor resolver
            var processorResolver = new ProcessorResolver();

            // resolve processors that implement iprocessor interface
            var processors = processorResolver
                .ResolveProcessors()
                .ToList();

            // verify test processor is resolved
            Assert.AreNotEqual(0, processors.Count);
            var testProcessor = processors
                .FirstOrDefault(x => x.Name.Equals(typeof(TestProcessor).Name));
            Assert.IsNotNull(testProcessor);
        }
    }
}