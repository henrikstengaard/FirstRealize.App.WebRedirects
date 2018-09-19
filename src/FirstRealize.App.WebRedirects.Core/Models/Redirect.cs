using System;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Redirect : IComparable<Redirect>
    {
        public Url OldUrl { get; set; }
        public Url NewUrl { get; set; }

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
        public bool IsIdentical
        {
            get
            {
                return IsValid &&
                    OldUrl.Parsed.AbsoluteUri.Equals(
                        NewUrl.Parsed.AbsoluteUri);
            }
        }

        public int CompareTo(Redirect other)
        {
            var oldUrlCompared = OldUrl.Parsed.AbsoluteUri.CompareTo(
                other.OldUrl.Parsed.AbsoluteUri);

            return oldUrlCompared != 0
                ? oldUrlCompared
                : NewUrl.Parsed.AbsoluteUri.CompareTo(
                    other.NewUrl.Parsed.AbsoluteUri);
        }
    }
}