using H.Playground.NancyWebApi.Core.Auth;
using H.Playground.NancyWebApi.Core.Extensions;
using Nancy;
using System;

namespace H.Playground.NancyWebApi.Core
{
    public class UserModule : SecuredModule
    {
        public UserModule() : base("/user")
        {
            Get(
                path: "/",
                action: _ =>
                {
                    return Response.AsJson(Context.GetCurrentUserInfo());
                },
                condition: null,
                name: null
            );
        }
    }
}
