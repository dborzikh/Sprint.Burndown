using System;
using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueChangesModel : JiraModelBase, IHasIdentifier
    {
        public string Key { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}
