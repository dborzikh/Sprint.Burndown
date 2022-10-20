using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.Models;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Services
{
    public class SprintService : ISprintService
    {
        private static SprintPeriod DefaultSprintPeriod => SprintPeriod.Create("0", DateTime.Today.AddDays(+30), DateTime.Today.AddDays(+30+14));

        private ICacheStorage CacheStorage { get; }

        private ICalendarService CalendarService { get; }

        private IChartService ChartService { get; }

        private IGlobalContext Context { get; }

        private IIssueService IssueService { get; }

        private IRepository Repository { get; }

        public SprintService(IGlobalContext context, ICalendarService calendarService, IChartService chartService, IRepository repository, ICacheStorage cacheStorage, IIssueService issueService)
        {
            CacheStorage = cacheStorage;
            CalendarService = calendarService;
            ChartService = chartService;
            Context = context;
            IssueService = issueService;
            Repository = repository;
        }

        public IList<SprintViewModel> GetSprints(BoardViewModel board)
        {
            var sprints = Repository.GetSprints(board.Id)
                .ProjectTo<SprintViewModel>()
                .OrderByDescending(s => s.IsFavorite)
                .ThenBy(s => s.Name)
                .ToList();

            sprints.ForEach(UpdatePercentPassed);

            return sprints;
        }

        private void UpdatePercentPassed(SprintViewModel sprint)
        {
            if (!sprint.StartDate.HasValue && !sprint.EndDate.HasValue)
            {
                var actualPeriod = GetActualDevelopmentDates(sprint);
                if (actualPeriod != DefaultSprintPeriod)
                {
                    sprint.StartDate = actualPeriod.BeginDate;
                    sprint.EndDate = actualPeriod.EndDate;
                }
            }

            if (!sprint.StartDate.HasValue || !sprint.EndDate.HasValue || sprint.StartDate > DateTime.Today)
            {
                return;
            }

            if (sprint.EndDate > DateTime.Today)
            {
                var daysTotal = (sprint.EndDate.Value - sprint.StartDate.Value).TotalDays;
                var daysLeft = (sprint.EndDate.Value - DateTime.Today).TotalDays;
                var daysPassed = daysTotal - daysLeft;
                sprint.PercentPassed = Convert.ToInt32((float)daysPassed / (float)daysTotal * 100f);
            }
            else
            {
                sprint.PercentPassed = 100;
            }
        }

        public SprintReportViewModel GetSprintReport(SprintViewModel sprint)
        {
            var isLaterThan = new Func<DateTime?, DateTime, bool>((date1, date2) => date1.HasValue && date1.Value.Date > date2.Date);

            var jiraSprint = Repository.GetSprintById(sprint.Id);
            var jiraBoard = Repository.GetBoardById(jiraSprint.OriginBoardId);

            var developDates = GetActualDevelopmentDates(sprint);
            var testingDates = GetActualTestingDates(sprint);
            var regressionDates = GetActualRegressionDates(sprint);

            var sprintName = jiraSprint.Name;
            var issues = IssueService.GetIssues(sprint);

            var devIssues = issues
                .Where(p => !p.IsQaTask())
                .ToArray();

            // Distinguish dept sub-tasks (already developed)
            foreach (var task in devIssues)
            {
                if (task.DevelopmentCompletedDate.HasValue && task.DevelopmentCompletedDate < developDates.BeginDate)
                {
                    task.Group = IssueGroup.Debt;
                }
            }

            // Update prioity of the sub-tasks from its parent tasks
            foreach (var childrenTasks in devIssues.Where(t => !string.IsNullOrEmpty(t.ParentId)).GroupBy(t => t.ParentId))
            {
                var parentTask = devIssues.FirstOrDefault(t => t.Id == childrenTasks.Key);
                if (parentTask != null)
                {
                    childrenTasks.ForEach(t =>
                    {
                        t.PriorityId = parentTask.PriorityId;
                        t.PriorityName = parentTask.PriorityName;
                        t.PriorityIconUrl = parentTask.PriorityIconUrl;
                    });
                }
            }

            // Fill-in IssueGroup on all sub-tasks based on Estimate and Team Velocity
            var teamVelocity = developDates.Velocity;
            
            var prioritizedTasks = devIssues
                .Where(t => t.Group != IssueGroup.Debt && t.HasDevEstimate())
                .OrderBy(t => t.BusinessPrioriry);
            
            int scopeEstimate = 0;
            foreach (var task in prioritizedTasks)
            {
                if (scopeEstimate < teamVelocity)
                {
                    task.Group = IssueGroup.Scope;
                    scopeEstimate += (task.DevelopmentEstimateSeconds / EstimatesExtension.SECONDS_IN_HOUR);
                }
                else
                {
                    task.Group = IssueGroup.Bonus;
                }
            }

            bool HasSubTasks(IssueViewModel t) => devIssues.Any(p => p.ParentId == t.Id);

            // Fill-in IssueGroup on all Parent tasks
            foreach (var parentTask in devIssues.Where(t => t.Group == IssueGroup.Undefined && HasSubTasks(t)))
            {
                var subTask = devIssues.FirstOrDefault(t => t.ParentId == parentTask.Id && t.Group != IssueGroup.Undefined);
                if (subTask != null)
                {
                    parentTask.Group = subTask.Group;
                }
            }

            devIssues = devIssues
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Key)
                .ToArray();

            var qaIssues = issues
                .Where(p => p.IsQaTask())
                .ToArray();

            var problemIssues = issues
                .Where(p => !p.IsQaTask() && p.HasImpediments())
                .ToArray();

            var unplannedIssues = issues
                .Where(p => p.IsDevTask() && isLaterThan(p.IncludedInSprintDate, developDates.BeginDate))
                .ToArray();

            var devChartIssues = issues
                .Where(p => p.IsDevTask())
                .Where(p => p.IsSubTask() || p.HasDevEstimate())
                .Where(p => p.Group != IssueGroup.Debt)
                .ToList();

            var testingChartIssues = issues
                .Where(p => p.IsDevTask())
                .Where(p => p.HasTestingEstimate())
                .ToList();

            var devChartData = ChartService.GenerateBurnDownSeriesFor(issue => issue.DevelopmentCompletedDate, issue => issue.DevelopmentEstimateSeconds, devChartIssues, developDates, sprintName);
            var crChartData = ChartService.GenerateBurnDownSeriesFor(p => p.CodeReviewCompletedDate, issue => issue.DevelopmentEstimateSeconds, devChartIssues, developDates, sprintName);
            var testingChartData = ChartService.GenerateBurnDownSeriesFor(p => p.TestingCompletedDate, issue => issue.TestingEstimateSeconds, testingChartIssues, testingDates, sprintName);
            var taskStatusSummaries = ChartService.GetTaskStatusSummaries(issues);

            return new SprintReportViewModel
            {
                BoardId = jiraSprint.OriginBoardId,
                BoardName = jiraBoard.Name,
                SprintId = sprint.Id,
                SprintName = sprintName,
                DevelopBeginDate = developDates.BeginDate,
                DevelopEndDate = developDates.EndDate,
                TestingBeginDate = testingDates.BeginDate,
                TestingEndDate = testingDates.EndDate,
                RegressionBeginDate = regressionDates.BeginDate,
                RegressionEndDate = regressionDates.EndDate,
                DevIssues = devIssues,
                QaIssues = qaIssues,
                ProblemIssues = problemIssues,
                UnplannedIssues = unplannedIssues,
                DevChartData = devChartData,
                CrChartData = crChartData,
                TestingChartData = testingChartData,
                DevTotals = CalculateTotalsFor(devIssues),
                QaTotals = CalculateTotalsFor(qaIssues),
                ProblemTotals = CalculateTotalsFor(problemIssues),
                UnplannedTotals = CalculateTotalsFor(unplannedIssues),
                TaskStatusSummaries = taskStatusSummaries,
                HasIncompleteData = Context.HasIncompleteData
            };
        }

        public SprintViewModel ToggleFavorite(SprintViewModel sprint)
        {
            var jiraSprint = Repository.GetSprintById(sprint.Id);
            jiraSprint.Additions.IsFavorite = sprint.IsFavorite;
            Repository.Save(jiraSprint.Additions);

            return Mapper.Map <SprintViewModel>(jiraSprint);
        }

        public void SaveChartPreview(ImageViewModel chartImage)
        {
            var chartPreview = new ChartPreviewModel
            {
                SprintId = chartImage.Id,
                ImageBody = chartImage.ImageBody
            };

            Repository.Save(chartPreview);
        }

        public ChartPreviewModel GetChartPreview(string sprintId)
        {
            var image = CacheStorage.Get<ChartPreviewModel>(sprintId);

            return image;
        }

        public void UpdateSprintDates(UpdateSprintDatesViewModel sprintDates)
        {
            var jiraSprint = Repository.GetSprintById(sprintDates.SprintId);

            switch (sprintDates.PeriodType)
            {
                case PeriodType.Development:
                    jiraSprint.Additions.DevelopBeginDate = sprintDates.BeginDate;
                    jiraSprint.Additions.DevelopEndDate = sprintDates.EndDate;
                    break;

                case PeriodType.Testing:
                    jiraSprint.Additions.TestingBeginDate = sprintDates.BeginDate;
                    jiraSprint.Additions.TestingEndDate = sprintDates.EndDate;
                    break;

                case PeriodType.Regression:
                    jiraSprint.Additions.RegressionBeginDate = sprintDates.BeginDate;
                    jiraSprint.Additions.RegressionEndDate = sprintDates.EndDate;
                    break;
            }

            Repository.Save(jiraSprint.Additions);
        }

        private SprintPeriod GetActualDevelopmentDates(SprintViewModel sprint)
        {
            switch (int.Parse(sprint.Id))
            {
                case 0001:
                    return SprintPeriod.Create(sprint.Id, new DateTime(2021,2,1), new DateTime(2021, 2, 14));
                default:
                    return DefaultSprintPeriod;
            }
        }

        private PeriodOfDates GetActualTestingDates(SprintViewModel sprint)
        {
            switch (int.Parse(sprint.Id))
            {
                case 0001:
                    return SprintPeriod.Create(sprint.Id, new DateTime(2021, 2, 7), new DateTime(2021, 2, 21));
                default:
                    return DefaultSprintPeriod;
            }
        }

        private PeriodOfDates GetActualRegressionDates(SprintViewModel sprint)
        {
            switch (int.Parse(sprint.Id))
            {
                case 0001:
                    return SprintPeriod.Create(sprint.Id, new DateTime(2021, 2, 22), new DateTime(2021, 2, 27));
                default:
                    var featureTesting = GetActualTestingDates(sprint);
                    var firstSprintDay = CalendarService.GetNearestWorkDay(featureTesting.EndDate.AddDays(1));
                    var lastSprintDay = CalendarService.GetNearestWorkDay(firstSprintDay.AddDays(5));
                    return SprintPeriod.Create(sprint.Id, firstSprintDay, lastSprintDay);
            }
        }

        private SprintEstimatesViewModel CalculateTotalsFor(IEnumerable<IssueViewModel> issues)
        {
            var totals = issues.DefaultIfEmpty(
                new IssueViewModel
                {
                    DevelopmentEstimateSeconds = 0,
                    TestingEstimateSeconds = 0,
                    TimeSpentSeconds = 0
                })
            .GroupBy(t => 1)
            .Select(g => new
            {
                DevelopmentEstimateSeconds = g.Sum(t => (int?)t.DevelopmentEstimateSeconds) ?? 0,
                TestingEstimateSeconds = g.Sum(t => (int?)t.TestingEstimateSeconds) ?? 0,
                TimeSpentSeconds = g.Sum(t => (int?)t.TimeSpentSeconds) ?? 0
            })
            .Single();

            return new SprintEstimatesViewModel
            {
                IssuesCount = issues.Count(),
                DevelopmentEstimateSeconds = totals.DevelopmentEstimateSeconds,
                TestingEstimateSeconds = totals.TestingEstimateSeconds,
                TimeSpentSeconds = totals.TimeSpentSeconds
            };
        }
    }
}
