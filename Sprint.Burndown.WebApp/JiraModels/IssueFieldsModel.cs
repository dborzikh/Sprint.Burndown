using System;
using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueFieldsModel
    {
        public IssueModel Parent { get; set; }

        public List<FixVersionModel> FixVersions { get; set; }

        public PriorityModel Priority { get; set; }

        public List<String> Labels { get; set; }

        public UserModel Assignee { get; set; }

        public StatusModel Status { get; set; }

        public List<ComponentModel> Components { get; set; }

        public UserModel Reporter { get; set; }

        public IssueTypeModel IssueType { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public string Description { get; set; }

        public TimeTrackingModel TimeTracking { get; set; }

        public string Summary { get; set; }

        public string customfield_15145 { get; set; }
        
        public decimal ActualTestTimeInDays
        {
            get
            {
                decimal.TryParse(customfield_15145, out var timeValue);
                return timeValue;
            }
        }
    }
}
