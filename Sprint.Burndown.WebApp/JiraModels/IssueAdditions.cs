using System;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.JiraModels
{
    [CacheEntity("Additions:Issue")]
    public class IssueAdditions : IHasIdentifier
    {
        public string Id { get; set; }

        public bool IsFavorite { get; set; }

        public IssueTransitionModel IncludedInSprint { get; set; }

        public IssueTransitionModel TechnicalAnalysisCompleted { get; set; }

        public IssueTransitionModel DevelopmentCompleted { get; set; }

        public IssueTransitionModel СodeReviewCompleted { get; set; }

        public IssueTransitionModel TestingCompleted { get; set; }

    }
}
