using FirstRealize.App.WebRedirects.Core.Models.Redirects;

namespace FirstRealize.App.WebRedirects.Core.Builders
{
    public interface IOutputRedirectBuilder
    {
        OutputRedirect Build(
            IProcessedRedirect processedRedirect);
    }
}