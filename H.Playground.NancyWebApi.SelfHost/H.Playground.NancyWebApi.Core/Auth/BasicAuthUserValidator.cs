using Nancy.Authentication.Basic;
using System.Security.Claims;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class BasicAuthUserValidator : IUserValidator
    {
        public const string clientIdClaimName = "ClientID";
        public const string clientSecretClaimName = "ClientSecret";

        public ClaimsPrincipal Validate(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            if (username != "hintee")
                return null;

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity[] {
                new ClaimsIdentity(
                    new UserIdentity(username),
                    new Claim[]
                    {
                        new Claim(clientIdClaimName, username),
                        new Claim(clientSecretClaimName, password)
                    }
                )
            });

            return principal;
        }
    }
}
