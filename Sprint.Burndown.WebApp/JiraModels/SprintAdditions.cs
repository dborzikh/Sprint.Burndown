using System;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.JiraModels
{
    [CacheEntity("Additions:Sprint")]
    public class SprintAdditions : IHasIdentifier
    {
        public string Id { get; set; }

        public DateTime? DevelopBeginDate { get; set; }

        public DateTime? DevelopEndDate { get; set; }

        public DateTime? TestingBeginDate { get; set; }

        public DateTime? TestingEndDate { get; set; }

        public DateTime? RegressionBeginDate { get; set; }

        public DateTime? RegressionEndDate { get; set; }

        public bool IsFavorite { get; set; }

        public int RecentIssuesCount { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
