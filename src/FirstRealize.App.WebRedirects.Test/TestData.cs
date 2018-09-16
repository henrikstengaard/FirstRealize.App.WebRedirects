using FirstRealize.App.WebRedirects.Core.Models;
using System;
using System.Collections.Generic;

namespace FirstRealize.App.WebRedirects.Test
{
    class TestData
    {
        public static Uri DefaultHost =
            new Uri("http://www.test.local");

        public static IEnumerable<Redirect> GetRedirects()
        {
            return new List<Redirect>
            {
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/example/path"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/new-url"
                    }
                },
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/new-url"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/another/path"
                    }
                },
                // causes duplicate redirect
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/new-url"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/redirect/somwhere/else"
                    }
                },
                // causes cyclic redirect
                new Redirect
                {
                    OldUrl = new Url
                    {
                        Raw = "/redirect/cyclic"
                    },
                    NewUrl = new Url
                    {
                        Raw = "/new-url"
                    }
                }
            };
        }
    }
}
