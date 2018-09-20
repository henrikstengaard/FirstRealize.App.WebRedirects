using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class NewUrlDomainReport : UrlDomainReportBase<NewUrlDomainRecord>
    {
        protected override void IndexUrlDomain(IParsedRedirect parsedRedirect)
        {
            if (_urlDomainsIndex.Contains(
                    parsedRedirect.NewUrl.Parsed.Host))
            {
                return;
            }

            _urlDomainsIndex.Add(
                parsedRedirect.NewUrl.Parsed.Host);
        }

        public override IEnumerable<NewUrlDomainRecord> GetRecords()
        {
            return _urlDomainsIndex
                .ToList()
                .OrderBy(x => x)
                .Select(x => new NewUrlDomainRecord
                {
                    NewUrlDomain = x
                });
        }
    }
}