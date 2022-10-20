using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class FixVersionModel : JiraModelBase, IHasIdentifier
    {
        public bool Archived { get; set; }

        public bool Released { get; set; }
    }
}
