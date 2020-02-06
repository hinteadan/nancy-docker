using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class Auth0Authenticator
    {
        const string rs256Token = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzFOMuIFdsOhs9f2n1sJE3e5LGIZEGHSGDXJIdy1ZvABet7JHWA2X8izfZr9kohAiNBrccXIV0rcygRD9Al/MVhTShqyqEjJGycIfo3m89BNFjagrzXCe5U9r3II4ZTC2LkN1KW4SLPOz0o9n4/8j8qQM/sB3bJVoLPCDaxax46rpOCyTdZ75FOAY+ON3addYncmlz6OkUUYUMwGJs6tfuwzuVTs0rWgR8V27eVr/u3K6L+0naBry0gKLjMcQchoN8iKOkEvHZ6WpsR57f7rWTObF7Z5NnKhSKdnKr7zZsZlGRhmTSjFPC2DoRG3lwAUf5LpPwFokBlpQt5w4xKOUIwIDAQAB";
        static readonly string userInfoUrl = $"https://{AuthSettings.Auth0Domain}/userinfo";

        public async Task<AuthTokenResult> GetApiAccessTokenFor(string clientID, string clientSecret)
        {
            using (var http = new HttpClient())
            using (HttpResponseMessage response
                = await http.PostAsync(
                    AuthSettings.Auth0TokenUrl,
                    new StringContent(
                        BuildTokenRequestBody(clientID, clientSecret),
                        Encoding.UTF8,
                        "application/json"
                    )
                )
            )
            {
                if (!response.IsSuccessStatusCode)
                    return AuthTokenResult.Fail();

                TokenRawResponse tokenRawResponse
                    = JsonConvert.DeserializeObject<TokenRawResponse>(await response.Content.ReadAsStringAsync());

                return AuthTokenResult.Win(tokenRawResponse.access_token, tokenRawResponse.expires_in);
            }
        }

        public async Task<UserInfo> GetUserInfo(string accessToken)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpResponseMessage response
                    = await http.GetAsync(userInfoUrl)
                )
                {
                    if (!response.IsSuccessStatusCode)
                        return null;

                    UserInfoRawResponse userInfoRawResponse
                        = JsonConvert.DeserializeObject<UserInfoRawResponse>(await response.Content.ReadAsStringAsync());

                    return
                        new UserInfo(Map(userInfoRawResponse));
                }
            }

        }

        internal Task GetUserInfo()
        {
            throw new NotImplementedException();
        }

        private static UserInfoDto Map(UserInfoRawResponse raw)
        {
            return
                new UserInfoDto
                {
                    UserID = raw.sub,
                    AvatarImageUrl = raw.picture,
                    Email = raw.email,
                    FullName = raw.name,
                    IsEmailVerified = raw.email_verified,
                    LastChangedAt = raw.updated_at,
                    Nickname = raw.nickname,
                };
        }

        public static RsaSecurityKey ConstructJwtSigningKey()
        {
            byte[] keyBytes = Convert.FromBase64String(rs256Token);
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            RsaSecurityKey signingKey = new RsaSecurityKey(rsa);
            return signingKey;
        }

        private string BuildTokenRequestBody(string clientID, string clientSecret)
        {
            return JsonConvert.SerializeObject(new
            {
                client_id = AuthSettings.Auth0ClientID,
                client_secret = AuthSettings.Auth0ClientSecret,
                audience = AuthSettings.Auth0ApiIdentifier,
                grant_type = AuthSettings.Auth0UserPassGrantType,
                username = clientID,
                password = clientSecret,
                realm = AuthSettings.Auth0UserPassConnection,
            });
        }

        class TokenRawResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }

        class UserInfoRawResponse
        {
            public string sub { get; set; }
            public string nickname { get; set; }
            public string name { get; set; }
            public string picture { get; set; }
            public DateTime updated_at { get; set; }
            public string email { get; set; }
            public bool email_verified { get; set; }
        }
    }
}
