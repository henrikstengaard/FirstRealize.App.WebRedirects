using System;
using System.Collections.Generic;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Results;
using FirstRealize.App.WebRedirects.Core.Processors;

namespace FirstRealize.App.WebRedirects.Test.EngineTests
{
    public class CauseErrorProcessor : IProcessor
    {
        public string Name => GetType().Name;

        public IEnumerable<IResult> Results => new List<IResult>();

        public void Process(IProcessedRedirect processedRedirect)
        {
            throw new InvalidOperationException(
                "Error caused for testing unknown error");
        }
    }
}