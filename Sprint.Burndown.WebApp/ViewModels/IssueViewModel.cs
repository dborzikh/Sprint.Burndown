using System;
using System.Diagnostics;
using System.Linq;

using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Extensions;

namespace Sprint.Burndown.WebApp.ViewModels
{
    [DebuggerDisplay("{Key} ({IssueTypeName}) {Description}")]
    public class IssueViewModel
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string ParentId { get; set; }

        public string ParentKey { get; set; }

        public string FixVersionId { get; set; }

        public string FixVersionName { get; set; }

        public string PriorityId { get; set; }

        public string PriorityName { get; set; }

        public string PriorityIconUrl { get; set; }

        public string[] Labels { get; set; }

        public string AssigneeId { get; set; }

        public string AssigneeName { get; set; }

        public string Tags => Group.ToString();

        public string TagsShortName => Group.GetShortName();

        public IssueGroup Group { get; set; }

        public string StatusId { get; set; }

        public String StatusName { get; set; }

        public String StatusIconUrl { get; set; }

        public String[] Components { get; set; }

        public string ReporterId { get; set; }

        public String ReporterName { get; set; }

        public String ReporterDisplayName { get; set; }

        public string IssueTypeId { get; set; }

        public String IssueTypeName { get; set; }

        public String IssueTypeIconUrl { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string[] FixVersions { get; set; }

        public int DevelopmentEstimateSeconds { get; set; }

        public string DevelopmentEstimate => DevelopmentEstimateSeconds.ToStringEstimate();

        public int TestingEstimateSeconds { get; set; }

        public string TestingEstimate => TestingEstimateSeconds.ToStringEstimate();

        public bool IsChildrenEstimates { get; set; }

        public string TimeSpent { get; set; }
      
        public int TimeSpentSeconds { get; set; }

        public DateTime? IncludedInSprintDate { get; set; }

        public DateTime? TechnicalAnalysisCompletedDate { get; set; }

        public DateTime? DevelopmentCompletedDate { get; set; }

        public DateTime? CodeReviewCompletedDate { get; set; }

        public DateTime? TestingCompletedDate { get; set; }
        public int BusinessPrioriry 
        { 
            get 
            {
                if (Labels != null && Labels.Any(p => p.Contains("bonus", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return 100;
                }

                return int.Parse(PriorityId);
            } 
        }

        public bool IsQaTask()
        {
            return JiraIssueType.QaTasks.Any(s => s.Equals(IssueTypeName, StringComparison.InvariantCultureIgnoreCase)) || Summary.Contains("[QA]");
        }

        public bool IsDevTask()
        {
            return !IsQaTask();
        }

        public bool IsSubTask()
        {
            return JiraIssueType.DevSubTasks.Any(s => s.Equals(IssueTypeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasImpediments()
        {
            return JiraIssueStatus.ImpedimentStatuses.Any(s => s.Equals(StatusName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasDevEstimate()
        {
            return DevelopmentEstimateSeconds > 0;
        }

        public bool HasTestingEstimate()
        {
            return TestingEstimateSeconds > 0;
        }
    }
}
