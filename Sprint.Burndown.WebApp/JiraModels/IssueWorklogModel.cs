using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueWorklogModel : JiraModelBase, IHasIdentifier
    {
        public string Key { get; set; }

        public PagedChangelogModel Worklog { get; set; }
    }
}
