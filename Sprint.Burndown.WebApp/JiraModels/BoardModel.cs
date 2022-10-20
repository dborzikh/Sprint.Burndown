using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.JiraModels
{
    [CacheEntity("Boards:All")]
    public class BoardModel : JiraModelBase, IHasIdentifier
    {
        public string Type { get; set; }
    }
}
