using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class UserModel : JiraModelBase
    {
        public string Key { get; set; }

        public string AccountId { get; set; }

        public string EmailAddress { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        public string TimeZone { get; set; }

        public Dictionary<string, string> AvatarUrls { get; set; }
    }
}
