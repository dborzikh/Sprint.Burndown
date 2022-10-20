using System;
using System.Collections.Generic;
using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueHistoryModel : IHasIdentifier
    {
        public string Id { get; set; }

        public UserModel Author { get; set; }

        public DateTime? Created { get; set; }

        public IList<IssueHistoryItemModel> Items { get; set; }
    }
}
