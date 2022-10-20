namespace Sprint.Burndown.WebApp.Core
{
    public class JiraOptions
    {
        public string JiraApiUrl { get; set; } = "https://testhost.jira.local/rest/";

        public string ActualTestTimeField { get; set; } = "customfield_15145";

        public bool UseBasicAuthentication { get; set; } = true;
    }
}
