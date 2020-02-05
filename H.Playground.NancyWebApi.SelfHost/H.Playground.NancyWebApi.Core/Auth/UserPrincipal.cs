using System;
using System.Linq;
using System.Security.Principal;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class UserPrincipal : IPrincipal
    {
        private readonly string[] roles = new string[0];

        public UserPrincipal(IIdentity identity, params string[] roles)
        {
            Identity = identity;
            this.roles = roles ?? new string[0];
        }

        public IIdentity Identity { get; }

        public bool IsInRole(string role)
        {
            return roles.Any(x => string.Equals(x, role, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
