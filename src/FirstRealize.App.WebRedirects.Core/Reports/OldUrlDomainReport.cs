﻿using FirstRealize.App.WebRedirects.Core.Models.Redirects;
using FirstRealize.App.WebRedirects.Core.Models.Reports;
using System.Collections.Generic;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public class OldUrlDomainReport : UrlDomainReportBase<OldUrlDomainRecord>
    {
        protected override void IndexUrlDomain(IParsedRedirect parsedRedirect)
        {
            if (_urlDomainsIndex.Contains(
                    parsedRedirect.OldUrl.Parsed.Host))
            {
                return;
            }

            _urlDomainsIndex.Add(
                parsedRedirect.OldUrl.Parsed.Host);
        }

        public override IEnumerable<OldUrlDomainRecord> GetRecords()
        {
            return _urlDomainsIndex
                .ToList()
                .OrderBy(x => x)
                .Select(x => new OldUrlDomainRecord
                {
                    OldUrlDomain = x
                });
        }
    }
}