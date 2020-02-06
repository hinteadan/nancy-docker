using H.Playground.NancyWebApi.Core.Auth;
using Microsoft.IdentityModel.Tokens;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Authentication.JwtBearer;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using H.Playground.NancyWebApi.Core.Extensions;

namespace H.Playground.NancyWebApi.Core
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly RsaSecurityKey jwtSigningKey = Auth0Authenticator.ConstructJwtSigningKey();

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Auth0Authenticator auth0Authenticator = container.Resolve<Auth0Authenticator>();

            base.ApplicationStartup(container, pipelines);

            pipelines.EnableJwtBearerAuthentication(
                new JwtBearerAuthenticationConfiguration
                {
                    //Challenge = "Guest",
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtSigningKey,

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = $"https://{AuthSettings.Auth0Domain}/",

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = AuthSettings.Auth0ApiIdentifier,

                        // Validate the token expiry
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero,

                        ValidateActor = true,
                    },
                }
            );

            pipelines.BeforeRequest.AddItemToEndOfPipeline(async (context, cancelToken) =>
            {
                if(context.CurrentUser != null)
                {
                    UserInfo userInfo = await auth0Authenticator.GetUserInfo(FetchBearerToken(context));

                    if (userInfo != null)
                    {
                        context.SetCurrentUserInfo(userInfo);
                        context.CurrentUser.AddIdentity(new ClaimsIdentity(userInfo.ToClaims()));
                    }
                }

                return null;
            });

            pipelines.AfterRequest.AddItemToEndOfPipeline(context =>
            {
                Console.WriteLine(Print(context));
            });
        }

        private static string FetchBearerToken(NancyContext context)
        {
            //get the token from request header
            string token = context.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

            //whether the token value start with the challenge from configuration
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length);
            }

            return token;
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            if (context.Request.Path.StartsWith("/auth"))
            {
                pipelines.EnableBasicAuthentication(
                    new BasicAuthenticationConfiguration(
                        new Auth.BasicAuthUserValidator(),
                        "H-Playground-NancyWebApi",
                        UserPromptBehaviour.Never
                    )
                );
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/", @"/Content")
            );
        }

        private static string Print(NancyContext context)
        {
            StringBuilder printer = new StringBuilder();

            printer.AppendLine($"====== START @ {DateTime.Now} ======");
            printer.AppendLine($"Request: {context.Request.Method} {context.Request.Url}");
            printer.AppendLine($"Response: {(int)context.Response.StatusCode}[{context.Response.StatusCode}] {context.Response.ReasonPhrase}");
            printer.AppendLine($"====== END ======");
            printer.AppendLine();

            return printer.ToString();
        }

        static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
    }
}
