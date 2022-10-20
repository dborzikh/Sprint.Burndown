using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueChangelogModel : JiraModelBase, IHasIdentifier
    {
        public string Key { get; set; }

        public PagedChangelogModel Changelog { get; set; }
    }
}
