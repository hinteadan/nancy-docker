using Nancy;
using Nancy.Security;

namespace H.Playground.NancyWebApi.Core
{
    public class SecuredModule : NancyModule
    {
        public SecuredModule(string path) : base(path)
        {
            this.RequiresAuthentication();
        }

        public SecuredModule() : this(null) { }
    }
}
