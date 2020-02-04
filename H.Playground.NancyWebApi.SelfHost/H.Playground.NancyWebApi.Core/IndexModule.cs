using Nancy;
using System;

namespace H.Playground.NancyWebApi.Core
{
    public class IndexModule : NancyModule
    {
        public IndexModule() : base()
        {
            Get(
                path: "/",
                action: _ => $"Hello Multiverse ! @ {DateTime.Now}",
                condition: null,
                name: null
            );
        }
    }
}
