using System;
using System.Linq;

namespace FirstRealize.App.WebRedirects.Core.Models.Urls
{
	public class ParsedUrl : IParsedUrl
	{
		public string Scheme { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string OriginalUrl { get; set; }
        public bool OriginalUrlHasHost { get; set; }


        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Scheme) &&
                    !string.IsNullOrWhiteSpace(Host) &&
                    Port > 0 &&
                    IsQueryStringValid(Query);
            }
        }

		public string PathAndQuery
		{
			get
			{
				return string.Concat(
					Path,
					!string.IsNullOrWhiteSpace(Query)
					? string.Concat("?", Query)
					: string.Empty);
			}
		}

        private bool IsQueryStringValid(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return true;
            }

            var queryParameters = queryString
                .Split('&')
                .ToList();

            return queryParameters.All(
                x => x.IndexOf("=", StringComparison.InvariantCultureIgnoreCase) >= 1);
        }

        public int CompareTo(object obj)
        {
            var other = obj as IParsedUrl;

            if (other == null ||
                !other.IsValid)
            {
                return 1;
            }

            if (!IsValid)
            {
                return -1;
            }

            var schemeCompared = Scheme.CompareTo(
                other.Scheme);

            if (schemeCompared != 0)
            {
                return schemeCompared;
            }

            var hostCompared = Host.CompareTo(
                other.Host);

            if (hostCompared != 0)
            {
                return hostCompared;
            }

            var portCompared = Port.CompareTo(
                other.Port);

            if (portCompared != 0)
            {
                return portCompared;
            }

            return (PathAndQuery ?? string.Empty).CompareTo(
                other.PathAndQuery ?? string.Empty);
        }
    }
}