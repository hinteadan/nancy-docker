using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class Auth0Authenticator
    {
        public async Task<AuthTokenResult> GetAccessTokenFor(string clientID, string clientSecret)
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

        private string BuildTokenRequestBody(string clientID, string clientSecret)
        {
            return JsonConvert.SerializeObject(new
            {
                client_id = AuthSettings.Auth0ClientID,
                client_secret = AuthSettings.Auth0ClientSecret,
                audience = AuthSettings.Auth0ApiIdentifier,
                grant_type = "client_credentials",
            });
        }

        class TokenRawResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}
