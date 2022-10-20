using System;
using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueTransitionModel : IHasIdentifier
    {
        public string Id { get; set; }

        public UserModel Author { get; set; }

        public DateTime? Created { get; set; }

        public String StatusName { get; set; }
    }
}
