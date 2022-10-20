using System.Linq;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class EmailExtensions
    {
        public static string ExtractUsername(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return email;
            }

            var parts = email.Split("@");

            return parts.First();
        }
    }
}
