namespace FirstRealize.App.WebRedirects.Core.Models.Urls
{
	public interface IParsedUrl
	{
		string Scheme { get; }
		string Host { get; }
		int Port { get; }
		string PathAndQuery { get; }
        string Path { get; }
        string Query { get; }
        string OriginalUrl { get; }
		bool IsValid { get; }
	}
}