using System;

using Sprint.Burndown.WebApp.Extensions;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class SprintEstimatesViewModel
    {
        public int IssuesCount { get; set; }

        public string DevelopmentEstimate => DevelopmentEstimateSeconds.ToStringEstimate();

        public string TestingEstimate => TestingEstimateSeconds.ToStringEstimate();

        public string TimeSpent => TimeSpentSeconds.ToStringEstimate();

        public int DevelopmentEstimateSeconds { get; set; }

        public int TestingEstimateSeconds { get; set; }

        public int TimeSpentSeconds { get; set; }
    }
}
