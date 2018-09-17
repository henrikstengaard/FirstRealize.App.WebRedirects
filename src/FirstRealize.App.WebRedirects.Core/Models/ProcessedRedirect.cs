using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Core.Models
{
    public class ProcessedRedirect : IProcessedRedirect
    {
        public Redirect Redirect { get; set; }
        public IList<IResult> Results { get; set; }

        public ProcessedRedirect()
        {
            Results = new List<IResult>();
        }
    }
}