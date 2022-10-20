using System;

using Sprint.Burndown.WebApp.Services;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class TaskStatusSummaryViewModel
    {
        public TaskStatusGroup Group { get; set; }

        public string Title { get; set; }

        public int IndicatorValue { get; set; }

        public NameValueViewModel[] SerieValues { get; set; }
    }
}
