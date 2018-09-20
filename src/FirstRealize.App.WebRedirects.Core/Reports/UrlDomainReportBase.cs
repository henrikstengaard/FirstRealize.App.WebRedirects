using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public abstract class UrlDomainReportBase<T> : ReportBase<T> where T : class
    {
        protected readonly HashSet<string> _urlDomainsIndex;

        public UrlDomainReportBase()
        {
            _urlDomainsIndex = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        protected abstract void IndexUrlDomain(
            IParsedRedirect parsedRedirect);

        public override void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach (var parsedRedirect in redirectProcessingResult
                .ParsedRedirects)
            {
                if (!parsedRedirect.IsValid)
                {
                    continue;
                }

                IndexUrlDomain(parsedRedirect);
            }
        }
    }
}