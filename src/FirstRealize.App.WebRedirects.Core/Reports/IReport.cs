using FirstRealize.App.WebRedirects.Core.Engines;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Reports
{
    public interface IReport<TEntity> where TEntity : class
    {
        void Build(IRedirectProcessingResult redirectEngine);
        IEnumerable<TEntity> GetRecords();
    }
}