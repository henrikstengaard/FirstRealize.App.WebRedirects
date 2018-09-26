using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Urls
{
	public class ParsedUrl : IParsedUrl
	{
		public string Scheme { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string PathAndQuery { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string OriginalUrl { get; set; }
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Scheme) &&
					!string.IsNullOrWhiteSpace(Host) &&
					Port > 0;
			}
		}
	}
}