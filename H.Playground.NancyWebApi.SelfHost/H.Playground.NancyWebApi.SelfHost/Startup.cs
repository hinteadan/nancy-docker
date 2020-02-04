using Owin;

namespace H.Playground.NancyWebApi.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}
