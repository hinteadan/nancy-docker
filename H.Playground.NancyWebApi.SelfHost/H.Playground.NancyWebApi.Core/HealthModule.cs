using Nancy;
using System;

namespace H.Playground.NancyWebApi.Core
{
    public class HealthModule : NancyModule
    {
        public HealthModule() : base("/health")
        {
            Get(
                path: "/ping",
                action: _ => $"Pong @ {DateTime.Now}",
                condition: null,
                name: null
            );
        }
    }
}
