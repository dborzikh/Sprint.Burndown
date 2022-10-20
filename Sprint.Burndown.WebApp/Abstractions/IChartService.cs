using System;
using System.Collections.Generic;

using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IChartService
    {
        SprintChartViewModel GenerateBurnDownSeriesFor(
            Func<IssueViewModel, DateTime?> dateSelector,
            Func<IssueViewModel, int> estimateSelector,
            IList<IssueViewModel> workIssues,
            PeriodOfDates sprintDates,
            string sprintName);

        HashSet<DateTime> GetPeriodWorkDays(PeriodOfDates sprintDates);

        TaskStatusSummaryViewModel[]  GetTaskStatusSummaries(IList<IssueViewModel> issues);
    }
}
