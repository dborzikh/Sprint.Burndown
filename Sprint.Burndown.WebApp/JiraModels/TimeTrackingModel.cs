using System;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class TimeTrackingModel
    {
        public String OriginalEstimate { get; set; }

        public String RemainingEstimate { get; set; }

        public String TimeSpent { get; set; }

        public int OriginalEstimateSeconds { get; set; }

        public int RemainingEstimateSeconds { get; set; }

        public int TimeSpentSeconds { get; set; }
    }
}
