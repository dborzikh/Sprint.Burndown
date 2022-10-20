using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;

namespace Sprint.Burndown.WebApp.Services
{
    public class CredentialsStorage : ICredentialsStorage
    {
        private static readonly IDictionary<string,UserCredentials> Credentials = new ConcurrentDictionary<string, UserCredentials>();

        private IHttpContextAccessor HttpContextAccessor { get; }

        public CredentialsStorage(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public void Add(string token, UserCredentials credentials)
        {
            Credentials.TryAdd(token, credentials);
        }

        public bool Exists(string token)
        {
            return Credentials.ContainsKey(token);
        }

        public void Remove(string token)
        {
            Credentials.Remove(token);
        }

        public UserCredentials GetCurrentUserCredentials()
        {
            var authorizationToken = HttpContextAccessor.HttpContext.GetAuthorizationToken();
            Credentials.TryGetValue(authorizationToken, out var credentials);

            return credentials;
        }
    }
}
