using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;

namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public class RedirectProcessedEventArgs : EventArgs
    {
        public IProcessedRedirect ProcessedRedirect { get; private set; }

        public RedirectProcessedEventArgs(
            IProcessedRedirect processedRedirect)
        {
            ProcessedRedirect = processedRedirect;
        }
    }
}