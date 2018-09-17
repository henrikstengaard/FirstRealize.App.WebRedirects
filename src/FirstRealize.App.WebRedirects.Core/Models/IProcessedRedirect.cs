using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public interface IProcessedRedirect
    {
        Redirect Redirect { get; }
        IList<IResult> Results { get; }
    }
}