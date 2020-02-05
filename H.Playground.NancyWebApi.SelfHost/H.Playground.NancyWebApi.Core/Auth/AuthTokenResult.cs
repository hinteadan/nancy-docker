using System;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class AuthTokenResult
    {
        private AuthTokenResult(bool isSuccessful, string token, DateTime asOf, TimeSpan expiresIn)
        {
            IsSuccessful = isSuccessful;
            Token = token;
            AsOf = asOf;
            ExpiresIn = expiresIn;
        }

        public bool IsSuccessful { get; }
        public DateTime AsOf { get; }
        public string Token { get; }
        public TimeSpan ExpiresIn { get; }

        public static AuthTokenResult Win(string token, int expiresInSeconds)
            => new AuthTokenResult(true, token, DateTime.Now, TimeSpan.FromSeconds(expiresInSeconds));

        public static AuthTokenResult Fail()
            => new AuthTokenResult(false, null, DateTime.Now, TimeSpan.Zero);
    }
}
