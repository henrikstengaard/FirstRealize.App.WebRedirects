using System;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class Url
    {
        public string Raw { get; set; }
        public Uri Parsed { get; set; }
        public bool IsValid
        {
            get
            {
                return Parsed != null;
            }
        }
        public bool HasHost
        {
            get
            {
                return IsValid && 
                    !string.IsNullOrEmpty(Parsed.DnsSafeHost);
            }
        }
    }
}