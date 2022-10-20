using System;
using System.Collections.Generic;
using System.Linq;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Services
{
    public class ChartService : IChartService
    {
        private readonly ICalendarService _calendarService;

        public ChartService(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public SprintChartViewModel GenerateBurnDownSeriesFor(
            Func<IssueViewModel,DateTime?> dateSelector,
            Func<IssueViewModel, int> estimateSelector,
            IList<IssueViewModel> workIssues, 
            PeriodOfDates sprintDates,
            string sprintName)
        {
            var factWorkDays = GetFactWorkDays(workIssues, dateSelector);
            var planWorkDays = GetPeriodWorkDays(sprintDates);

            var totalDevEstimate = workIssues.Sum(p => (estimateSelector(p) / EstimatesExtension.SECONDS_IN_HOUR));

            var planDates = new PeriodOfDates
            {
                BeginDate = planWorkDays.Min(),
                EndDate = planWorkDays.Max()
            };

            var factDates= new PeriodOfDates
            {
                BeginDate = factWorkDays.DefaultIfEmpty(planDates.BeginDate).Min().LimitBySprintStart(planDates.BeginDate),
                EndDate = factWorkDays.DefaultIfEmpty(planDates.EndDate).Max()
            };

            var developWorkDays = workIssues
                .Where(p => dateSelector(p).HasValue)
                .Select(p => new
                {
                    WorkCompletedDate = dateSelector(p).Value.Date.LimitBySprintStart(planDates.BeginDate),
                    EstimateHours = estimateSelector(p) / EstimatesExtension.SECONDS_IN_HOUR,
                    p.Key
                })
                .GroupBy(k => k.WorkCompletedDate)
                .Select(g => new
                {
                    WorkDay = g.Key,
                    WorkHours = g.Sum(v => v.EstimateHours),
                    Tasks = g.Select(t => t.Key).ToArray()
                })
                .OrderBy(p => p.WorkDay)
                .ToList();

            // Adding horizontal parts on the Chart for days without Work done.
            var missedDays = planWorkDays
                .Except(developWorkDays.Select(p => p.WorkDay))
                .Where(day => day <= DateTime.Today);

            foreach (var missedDay in missedDays)
            {
                developWorkDays.Add(
                new
                {
                    WorkDay = missedDay,
                    WorkHours = 0,
                    Tasks = new String[0]
                });
            }

            var devChartData = developWorkDays
                .Select(p => new SeriesDataViewModel
                {
                    FactDate = p.WorkDay,
                    WorkHours = p.WorkHours,
                    WorkDone = totalDevEstimate - developWorkDays.Where(d => d.WorkDay <= p.WorkDay).Sum(d => d.WorkHours),
                    Tasks = p.Tasks.Select(task => 
                    new SeriesTaskViewModel
                    {
                        Key = task,
                        Url = $"https://testhost.jira.local/browse/{task}"
                    })
                    .ToList()
                })
                .OrderBy(p => p.FactDate)
                .ToList();

            var dayBeforeSprint = planDates.BeginDate.AddDays(-1);
            devChartData.Insert(0, new SeriesDataViewModel
            {
                FactDate = dayBeforeSprint,
                WorkHours = 0,
                WorkDone = totalDevEstimate,
                Tasks = new SeriesTaskViewModel[0]
            });

            var workDayNames = new[]
            {
                dayBeforeSprint
            }
            .Concat(planWorkDays)
            .Concat(factWorkDays)
            .Select(day => day.Date)
            .Distinct()
            .OrderBy(day => day)
            .Select(day => new
            {
                Name = day.ToString("d/MMM"),
                Date = day
            })
            .ToList();

            var categiries = workDayNames
                .Select(p => p.Name)
                .ToArray();

            devChartData.ForEach(p => p.Index = workDayNames.FirstIndexFor(day => day.Date == p.FactDate));

            var result = new SprintChartViewModel
            {
                SprintName = sprintName,
                Categories = categiries,
                PlanBeginDate = dayBeforeSprint,
                PlanEndDate = planDates.EndDate,
                PlanBeginIndex = workDayNames.FirstIndexFor(p => p.Date == dayBeforeSprint),
                PlanEndIndex = workDayNames.FirstIndexFor(p => p.Date == planDates.EndDate),

                FactBeginDate = dayBeforeSprint,
                FactEndDate = factDates.EndDate,
                FactBeginIndex = workDayNames.FirstIndexFor(p => p.Date == dayBeforeSprint),
                FactEndIndex = workDayNames.FirstIndexFor(p => p.Date == factDates.EndDate),
                TotalEstimate = totalDevEstimate,
                Data = devChartData
            };

            return result;
        }

        public HashSet<DateTime> GetPeriodWorkDays(PeriodOfDates sprintDates)
        {
            return sprintDates
                .Where(day => _calendarService.IsWorkDay(day))
                .Distinct()
                .OrderBy(day => day)
                .ToHashSet();
        }

        public TaskStatusSummaryViewModel[] GetTaskStatusSummaries(IList<IssueViewModel> issues)
        {
            var anaysisSummary = new TaskStatusSummaryViewModel
            {
                Group = TaskStatusGroup.Analysis,
                Title = "Analysis",
                SerieValues = new[]
                {
                    GetSummaryFor(JiraIssueStatus.Open, issues),
                    GetSummaryFor(JiraIssueStatus.Analysis, issues),
                    GetSummaryFor(JiraIssueStatus.WaitForAnalysis, issues),
                    GetSummaryFor(JiraIssueStatus.AnalysisApproval, issues)
                }
            };

            var developmentSummary = new TaskStatusSummaryViewModel
            {
                Group = TaskStatusGroup.Development,
                Title = "Development",
                IndicatorValue = GetSummaryFor(JiraIssueStatus.DevInProgress, issues).Value,
                SerieValues = new[]
                {
                    GetSummaryFor(JiraIssueStatus.DevInProgress, issues),
                    GetSummaryFor(JiraIssueStatus.WaitForDev, issues),
                    GetSummaryFor(JiraIssueStatus.PlannedInSprint, issues),
                    GetSummaryFor(JiraIssueStatus.CodeReview, issues),
                    GetSummaryFor(JiraIssueStatus.BugsFound, issues),
                    GetSummaryFor(JiraIssueStatus.WaitForMerge, issues)
                }
            };

            var testingSummary = new TaskStatusSummaryViewModel
            {
                Group = TaskStatusGroup.Testing,
                Title = "Testing",
                IndicatorValue = GetSummaryFor(JiraIssueStatus.TestInPorgress, issues).Value,
                SerieValues = new[]
                {
                    GetSummaryFor(JiraIssueStatus.TestInPorgress, issues),
                    GetSummaryFor(JiraIssueStatus.WaitForTests, issues),
                }
            };

            var doneSeries = issues.ToList()
                .Where(t => !anaysisSummary.SerieValues.Select(p => p.Name.ToUpper()).Contains(t.StatusName.ToUpper()))
                .Where(t => !developmentSummary.SerieValues.Select(p => p.Name.ToUpper()).Contains(t.StatusName.ToUpper()))
                .Where(t => !testingSummary.SerieValues.Select(p => p.Name.ToUpper()).Contains(t.StatusName.ToUpper()))
                .Select(t => t.StatusName)
                .Distinct()
                .Select(status => GetSummaryFor(status, issues))
                .ToArray();

            var doneSummary = new TaskStatusSummaryViewModel
            {
                Group = TaskStatusGroup.Done,
                Title = "Done",
                SerieValues = doneSeries
            };

            UpdateIndicatorValueOf(anaysisSummary);
            UpdateIndicatorValueOf(developmentSummary);
            UpdateIndicatorValueOf(testingSummary);
            UpdateIndicatorValueOf(doneSummary);

            var result = new[]
            {
                anaysisSummary,
                developmentSummary,
                testingSummary,
                doneSummary
            };

            return result;
        }

        private NameValueViewModel GetSummaryFor(string statusName, IList<IssueViewModel> issues)
        {
            var count = issues
                .Where(p => p.IsDevTask())
                .Count(p => string.Equals(p.StatusName, statusName, StringComparison.InvariantCultureIgnoreCase));

            return new NameValueViewModel(statusName, count);
        }

        private void UpdateIndicatorValueOf(TaskStatusSummaryViewModel summaryModel)
        {
            summaryModel.IndicatorValue = summaryModel.SerieValues
                .Select(p => (int?)p.Value)
                .Sum(p => p) ?? 0;

            if (summaryModel.IndicatorValue > 0)
            { 
                summaryModel.SerieValues = summaryModel.SerieValues
                    .Where(p => p.Value != 0)
                    .ToArray();
            }
        }

        private HashSet<DateTime> GetFactWorkDays(IList<IssueViewModel> workIssues, Func<IssueViewModel, DateTime?> dateSelector)
        {
            return workIssues
                .Where(p => dateSelector(p).HasValue)
                .Select(p => dateSelector(p).Value.Date)
                .Distinct()
                .OrderBy(day => day)
                .ToHashSet();
        }
    }
}
