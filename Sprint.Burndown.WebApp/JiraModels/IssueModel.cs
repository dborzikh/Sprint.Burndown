using System;
using System.Linq;

using Newtonsoft.Json;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.Const;

namespace Sprint.Burndown.WebApp.JiraModels
{
    [CacheEntity("Issues:All")]
    public class IssueModel : JiraModelBase, IHasIdentifier, IHasAdditions<IssueAdditions>
    {
        public string Key { get; set; }

        public SprintModel Sprint { get; set; }

        public IssueFieldsModel Fields { get; set; }

        [JsonIgnore]
        public IssueAdditions Additions { get; set; }

        public bool IsTestingSupported => JiraIssueType.DevSubTasks.All(s => !s.Equals(Fields?.IssueType?.Name, StringComparison.InvariantCultureIgnoreCase));

        public IHasIdentifier GetAdditions() => Additions;
    }
}
