using System;

namespace FirstRealize.App.WebRedirects.Core.Models.Urls
{
	public interface IParsedUrl : IComparable
    {
		string Scheme { get; set; }
		string Host { get; set; }
		int Port { get; set; }
		string PathAndQuery { get; set; }
        string Path { get; set; }
        string Query { get; set; }
        string OriginalUrl { get; set; }
        bool OriginalUrlHasHost { get; set; }
        bool IsValid { get; }
	}
}