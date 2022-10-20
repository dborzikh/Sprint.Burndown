using System;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class SprintReportViewModel
    {
        public string BoardId { get; set; }

        public string BoardName { get; set; }

        public string SprintId { get; set; }

        public string SprintName { get; set; }

        public DateTime DevelopBeginDate { get; set; }

        public DateTime DevelopEndDate { get; set; }

        public int DevelopSingleVelocity { get; set; }

        public DateTime TestingBeginDate { get; set; }

        public DateTime TestingEndDate { get; set; }

        public int TestingSingleVelocity { get; set; }

        public DateTime RegressionBeginDate { get; set; }

        public DateTime RegressionEndDate { get; set; }

        public IssueViewModel[] DevIssues { get; set; }

        public IssueViewModel[] QaIssues { get; set; }

        public IssueViewModel[] ProblemIssues { get; set; }

        public IssueViewModel[] UnplannedIssues { get; set; }

        public SprintChartViewModel DevChartData { get; set; }

        public SprintChartViewModel CrChartData { get; set; }

        public SprintChartViewModel TestingChartData { get; set; }

        public SprintEstimatesViewModel DevTotals { get; set; }

        public SprintEstimatesViewModel QaTotals { get; set; }

        public SprintEstimatesViewModel ProblemTotals { get; set; }

        public SprintEstimatesViewModel UnplannedTotals { get; set; }

        public TaskStatusSummaryViewModel[] TaskStatusSummaries { get; set; }

        public bool HasIncompleteData { get; set; }
    }
}
