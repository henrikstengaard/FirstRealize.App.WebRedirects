using FirstRealize.App.WebRedirects.Core.Engines;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class OldUrlDomainReport : IReport<OldUrlDomainRecord>
    {
        private readonly HashSet<string> _oldUrlDomainsIndex;

        public OldUrlDomainReport()
        {
            _oldUrlDomainsIndex = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
        }

        public void Build(
            IRedirectProcessingResult redirectProcessingResult)
        {
            foreach(var redirect in redirectProcessingResult
                .Redirects)
            {
                if (!redirect.IsValid ||
                    _oldUrlDomainsIndex.Contains(
                        redirect.OldUrl.Parsed.Host))
                {
                    continue;
                }

                _oldUrlDomainsIndex.Add(
                    redirect.OldUrl.Parsed.Host);
            }
        }

        public IEnumerable<OldUrlDomainRecord> GetRecords()
        {
            return _oldUrlDomainsIndex
                .ToList()
                .OrderBy(x => x)
                .Select(x => new OldUrlDomainRecord
                {
                    OldUrlDomain = x
                });
        }
    }
}