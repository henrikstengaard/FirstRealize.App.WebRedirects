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
                return OldUrl.IsValid &&
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
            var oldUrlCompared = other.OldUrl.Parsed.AbsoluteUri.CompareTo(
                OldUrl.Parsed.AbsoluteUri);

            return oldUrlCompared != 0
                ? oldUrlCompared
                : other.NewUrl.Parsed.AbsoluteUri.CompareTo(
                    NewUrl.Parsed.AbsoluteUri);
        }
    }
}