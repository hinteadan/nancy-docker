using H.Playground.NancyWebApi.Core.Auth;
using Nancy;
using System.Linq;

namespace H.Playground.NancyWebApi.Core
{
    public class AuthModule : NancyModule
    {
        public AuthModule(
            Auth0Authenticator auth0Authenticator
            ) : base("/auth")
        {
            Post(
                path: "/token",
                action: async (_, c) =>
                {
                    System.Security.Claims.ClaimsPrincipal user = Context.CurrentUser;

                    if (user == null)
                        return HttpStatusCode.Forbidden;

                    if (!user.Identity.IsAuthenticated)
                        return HttpStatusCode.Unauthorized;

                    AuthTokenResult tokenResult = await auth0Authenticator.GetApiAccessTokenFor(
                        user.Claims.Single(x => x.Type == BasicAuthUserValidator.clientIdClaimName).Value,
                        user.Claims.Single(x => x.Type == BasicAuthUserValidator.clientSecretClaimName).Value
                    );

                    if (!tokenResult.IsSuccessful)
                        return HttpStatusCode.Unauthorized;

                    return Response.AsJson(new { 
                        token = tokenResult.Token,
                        asOf = tokenResult.AsOf.ToString(),
                        expiresInSeconds = (int)tokenResult.ExpiresIn.TotalSeconds,
                    });
                },
                condition: null,
                name: null
            );
        }
    }
}
