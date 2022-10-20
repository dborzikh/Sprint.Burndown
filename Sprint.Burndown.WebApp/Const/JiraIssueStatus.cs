using System;

namespace Sprint.Burndown.WebApp.Const
{
    public static class JiraIssueStatus
    {
        public static readonly string[] ImpedimentStatuses =
        {
            WaitForMerge,
            Analysis,
            AnalysisApproval
        };

        public const string Open = "Open";

        public const string Closed = "Closed";

        public const string CodeReview = "Code Review";

        public const string Completed = "Completed";

        public const string DevInProgress = "Dev In Progress";

        public const string BugsFound = "Bugs Found";

        public const string WaitForTests = "Wait For Tests";

        public const string TestInPorgress = "Test In Porgress";

        public const string RegressionIssues = "Regression Issues";

        public const string IntegrationTests = "Integration Tests";

        public const string WaitForRelease = "Wait For Release";

        public const string WaitForUserAccept = "Wait For User Accept";

        public const string WaitForDev = "Wait For Development";

        public const string PlannedInSprint = "Planned In Sprint";

        public const string WaitForMerge = "Wait For Merge";

        public const string Analysis = "Analysis";

        public const string WaitForAnalysis = "Wait For Analysis";

        public const string AnalysisApproval = "Analysis Approval";

        public const string WaitForInformation = "Wait For Information";
    }
}
