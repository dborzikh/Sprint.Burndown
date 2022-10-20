using System;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueTypeModel : JiraModelBase
    {
        public string Description { get; set; }

        public string IconUrl { get; set; }

        public bool Subtask { get; set; }

        public int AvatarId { get; set; }
    }
}
