using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Redirects
{
    public class Url : IUrl
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
        public bool HasHost { get; set; }
    }
}