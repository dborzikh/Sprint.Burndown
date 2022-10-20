using Sprint.Burndown.WebApp.Utils;

namespace Sprint.Burndown.WebApp.Core
{
    public class UserCredentials
    {
        public string UserId => HashCodeUtility.GetPersistentHashCode(UserName).ToString();

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }
    }
}
