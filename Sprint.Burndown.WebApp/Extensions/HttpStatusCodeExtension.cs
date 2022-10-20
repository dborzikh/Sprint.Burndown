using RestSharp;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class HttpStatusCodeExtension
    {
        public static bool IsSuccessful(this IRestResponse response)
        {
            return ((int)response.StatusCode >= 200) && ((int)response.StatusCode <= 299);
        }
    }
}
