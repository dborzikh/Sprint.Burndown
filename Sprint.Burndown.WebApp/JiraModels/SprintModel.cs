using System;

using Newtonsoft.Json;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.Models;

namespace Sprint.Burndown.WebApp.JiraModels
{
    [CacheEntity("Sprints:All")]
    public class SprintModel : IHasIdentifier, IHasAdditions<SprintAdditions>
    {
        public string Id { get; set; }

        public string Self { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string OriginBoardId { get; set; }

        public string Goal { get; set; }

        [JsonIgnore]
        public SprintAdditions Additions { get; set; }

        public IHasIdentifier GetAdditions() => Additions;

        [JsonIgnore]
        public ChartPreviewModel ChartPreview { get; set; }

    }
}
