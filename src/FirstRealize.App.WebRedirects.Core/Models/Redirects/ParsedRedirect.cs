using FirstRealize.App.WebRedirects.Core.Models.Urls;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class ParsedRedirect : IParsedRedirect
    {
        public IUrl OldUrl { get; set; }
        public IUrl NewUrl { get; set; }
		public RedirectType RedirectType { get; set; }

        public bool IsValid
        {
            get
            {
                return OldUrl != null &&
                    OldUrl.IsValid &&
                    NewUrl != null &&
                    NewUrl.IsValid;
            }
        }

        public int CompareTo(object obj)
        {
            var other = obj as IParsedRedirect;

            if (other == null ||
                other.OldUrl == null ||
                other.NewUrl == null ||
                !other.IsValid)
            {
                return 1;
            }

            if (!IsValid)
            {
                return -1;
            }

            var oldUrlCompared = OldUrl.Parsed.CompareTo(
                other.OldUrl.Parsed);

            return oldUrlCompared != 0
                ? oldUrlCompared
                : NewUrl.Parsed.CompareTo(
                    other.NewUrl.Parsed);
        }
    }
}