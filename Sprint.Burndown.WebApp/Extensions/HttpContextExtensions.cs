using Microsoft.AspNetCore.Http;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class HttpContextExtensions
    {
        private const string AUTHORIZATION_HEADER = "Authorization";

        public static string GetAuthorizationToken(this HttpContext httpContext)
        {
            var bearerToken = httpContext?.Request.Headers[AUTHORIZATION_HEADER].ToString();
            var userToken = string.IsNullOrEmpty(bearerToken) 
                ? bearerToken 
                : bearerToken.Replace("Bearer ", string.Empty);

            return userToken;
        }
    }
}
