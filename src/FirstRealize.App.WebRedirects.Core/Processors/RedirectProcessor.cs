using FirstRealize.App.WebRedirects.Core.Models;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Processors
{
    public class RedirectProcessor
    {
        private readonly IDictionary<string, ProcessedRedirect> _oldUrlIndex;

        public RedirectProcessor()
        {
            _oldUrlIndex = new Dictionary<string, ProcessedRedirect>();
        }

        public ProcessedRedirect Process(
            Redirect redirect)
        {
            var processedRedirect = new ProcessedRedirect
            {
                Redirect = redirect
            };

            if (!redirect.IsValid ||
                redirect.IsIdentical)
            {
                return processedRedirect;
            }

            var oldUrl = redirect.OldUrl.Parsed.AbsoluteUri;
            if (_oldUrlIndex.ContainsKey(oldUrl))
            {
                _oldUrlIndex[oldUrl].IsDuplicate = true;
                _oldUrlIndex[oldUrl].Status =
                    string.Format(
                        "Duplicate redirect from old url '{0}'! Existing redirect to new url '{1}' replaced by redirect to new url '{2}'",
                        redirect.OldUrl.Parsed.AbsoluteUri,
                        _oldUrlIndex[oldUrl].Redirect.NewUrl.Parsed.AbsoluteUri,
                        redirect.NewUrl.Parsed.AbsoluteUri);
            }

            _oldUrlIndex[oldUrl] = processedRedirect;

            return processedRedirect;
        }
    }
}
