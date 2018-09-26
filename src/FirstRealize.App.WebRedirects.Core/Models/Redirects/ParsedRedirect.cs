using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class ParsedRedirect : IParsedRedirect, IComparable
    {
        public IUrl OldUrl { get; set; }
        public IUrl NewUrl { get; set; }

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
            var other = obj as ParsedRedirect;

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

            var oldUrlCompared = OldUrl.Parsed.AbsoluteUri.CompareTo(
                other.OldUrl.Parsed.AbsoluteUri);

            return oldUrlCompared != 0
                ? oldUrlCompared
                : NewUrl.Parsed.AbsoluteUri.CompareTo(
                    other.NewUrl.Parsed.AbsoluteUri);
        }
    }
}