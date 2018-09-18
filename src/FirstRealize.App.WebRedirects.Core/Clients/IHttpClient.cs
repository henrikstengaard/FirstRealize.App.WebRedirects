namespace FirstRealize.App.WebRedirects.Core.Clients
{
    public interface IHttpClient
    {
        HttpResponse Get(
            string url);
    }
}