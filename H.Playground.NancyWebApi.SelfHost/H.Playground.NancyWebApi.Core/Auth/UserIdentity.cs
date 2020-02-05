using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class UserIdentity : IIdentity
    {
        public UserIdentity(string name, bool isAuthenticated = true, string authenticationType = "Basic")
        {
            Name = name;
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;
        }

        public string AuthenticationType { get; } = null;

        public bool IsAuthenticated { get; } = false;

        public string Name { get; } = null;
    }
}
