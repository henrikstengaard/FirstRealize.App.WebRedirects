namespace FirstRealize.App.WebRedirects.Core.Engines
{
    public interface IRedirectEngine
    {
        IRedirectProcessingResult Run(
            bool process);
    }
}