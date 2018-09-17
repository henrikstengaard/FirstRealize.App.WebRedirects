namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Redirect
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


    }
}