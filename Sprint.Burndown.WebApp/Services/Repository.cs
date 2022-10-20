using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NLog;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.JiraModels;
using Sprint.Burndown.WebApp.Models;
using Sprint.Burndown.WebApp.Utils;

namespace Sprint.Burndown.WebApp.Services
{
    public class Repository : IRepository
    {
        private static readonly ILogger Logger = LoggerType.General;

        private IGlobalContext Context { get; }

        private IJiraFacade JiraFacade { get; }

        private ICacheStorage CacheStorage { get; }

        private ICredentialsStorage CredentialsStorage { get; }

        private INotificationService Notifications { get; set; }

        private Lazy<Session> Session { get; }

        public Repository(IGlobalContext context, IJiraFacade jiraFacade, ICacheStorage cacheStorage, INotificationService notifications, ICredentialsStorage credentialsStorage)
        {
            Context = context;
            CacheStorage = cacheStorage;
            CredentialsStorage = credentialsStorage;
            JiraFacade = jiraFacade;
            Notifications = notifications;

            var credentials = CredentialsStorage.GetCurrentUserCredentials();
            Session = new Lazy<Session>(() => JiraFacade.Login(credentials.UserName, credentials.Password));
        }

        public IEnumerable<BoardModel> GetBoards()
        {
            var boards = JiraFacade.GetBoards(Session)
                .FromCache(CacheKeys.AllBoards(), out var hasPendingItems)
                .OrderBy(p => p.Name)
                .ToList();

            Context.HasIncompleteData = hasPendingItems;

            return boards;
        }
        
        public BoardModel GetBoardById(string boardId)
        {
            var board = JiraFacade.GetBoard(Session, boardId)
                .FromCacheById(CacheKeys.AllBoards(), boardId);

            return board;
        }

        public IEnumerable<SprintModel> GetSprints(string boardId)
        {
            var sprints = JiraFacade.GetSprints(Session, boardId)
                .FromCache(CacheKeys.SprintsOf(boardId), out var hasPendingItems)
                .OrderBy(p => p.Name)
                .ToList();

            Context.HasIncompleteData = hasPendingItems;
            sprints.Combine(CacheStorage.Get<SprintAdditions>(sprints.Select(p => p.Id)));

            var defaultChartPreview = CacheStorage.Get<ChartPreviewModel>("-1");
            if (defaultChartPreview == null)
            {
                var defaultSvgChart = string.Join(string.Empty, File.ReadAllLines("./wwwroot/images/empty-chart.svg"));
                defaultChartPreview = new ChartPreviewModel
                {
                    Id = "-1",
                    ImageBody = defaultSvgChart
                };
            }

            var previews = CacheStorage
                .Get<ChartPreviewModel>(sprints.Select(p => p.Id))
                .ToDictionary(key => key.SprintId, val => val);

            
            sprints.ForEach(sprint => sprint.ChartPreview = previews.GetValueOrDefault(sprint.Id, defaultChartPreview));

            return sprints;
        }

        public SprintModel GetSprintById(string sprintId)
        {
            var sprint = JiraFacade.GetSprint(Session, sprintId)
                .FromCacheById(CacheKeys.AllSprints, sprintId);

            sprint.Combine(CacheStorage.Get<SprintAdditions>(sprint.Id));

            return sprint;
        }
        
        public IEnumerable<IssueModel> GetIssues(string sprintId)
        {
            var cacheContext = RedisCacheExtensions.GetHttpCacheContext();
            var issuesCacheInfo = CacheKeys.IssuesOf(sprintId);

            var needsToBeReloaded = cacheContext.InvalidateCache;

            // Should be get _before_ the cache renewal to perform a partial update
            var changedSinceLastUpdate = needsToBeReloaded 
                ? GetUpdatedIssues(sprintId).ToList()
                : new List<IssueModel>(0);

            var sprint = JiraFacade
                .GetSprint(Session, sprintId)
                .FromCacheById(CacheKeys.AllIssues, sprintId);

            var issues = JiraFacade
                .GetIssues(Session, sprintId)
                .FromCache(issuesCacheInfo, out var hasPendingItems)
                .OrderBy(p => p.Key)
                .ToList();

            var additions = CacheStorage.Get<IssueAdditions>(issues.Select(p => p.Id));
            issues.Combine(additions);
            issues.ForEach(issue => issue.Sprint = sprint);

            Context.HasIncompleteData = hasPendingItems;

            if (needsToBeReloaded)
            {
                if (!changedSinceLastUpdate.Any() || !cacheContext.AllowPartialUpdate)
                {
                    // perform a full update
                    changedSinceLastUpdate = issues;
                }

                // read all updated issues from scratch
                var reloadedIssues = JiraFacade.GetIssues(Session, sprintId).Value.Values
                    .Where(issue => changedSinceLastUpdate.Any(changed => changed.Id == issue.Id))
                    .ToList();

                reloadedIssues.Combine(additions);
                reloadedIssues.ForEach(issue => issue.Sprint = sprint);

                var processed = 1;
                var totalIssues = reloadedIssues.Count;
                Notifications.LoadingStarted(totalIssues, sprintId.ToString());

                foreach (var issue in reloadedIssues)
                {
                    var issueChangelog = JiraFacade.GetIssueChangelog(Session, issue.Id).Value;

                    UpdateTransitionDates(issue, issueChangelog.Changelog);
                    Save(issue, issuesCacheInfo);

                    Notifications.LoadingProcessed(processed += 1);
                }

                Notifications.LoadingFinished();
            }

            return issues; 
        }

        public IEnumerable<IssueModel> GetUpdatedIssues(string sprintId)
        {
            var issuesCacheInfo = CacheKeys.IssuesOf(sprintId);
            var oldIssues = CacheStorage.GetAll<IssueModel>(issuesCacheInfo.Key);

            if (oldIssues == null || !oldIssues.Any())
            {
                return new IssueModel[0];
            }

            var issuesUpdateDates = JiraFacade.GetIssueUpdates(Session, sprintId);

            var addedIssues = issuesUpdateDates
                .Where(issue => oldIssues.All(t => t.Id != issue.Key))
                .Select(issue => new IssueModel { Id = issue.Key })
                .ToList();
            
            var removedIssues = oldIssues
                .Where(issue => issuesUpdateDates.All(t => t.Key != issue.Id))
                .ToList();

            var updatedIssues = oldIssues
                .Where(issue => HasChangesInJira(issue, issuesUpdateDates))
                .ToDictionary(issue => issue.Key, issue => issue.Fields.Updated);

            var result = oldIssues
                .Where(issue => updatedIssues.ContainsKey(issue.Key))
                .Concat(addedIssues)
                .Concat(removedIssues)
                .ToList();

            return result;
        }

        private void CopySprintPropertiesFromParent(IList<IssueModel> issues)
        {
            var subTasks = issues.Where(p => p.Additions.IncludedInSprint == null);
            foreach (var subTask in subTasks)
            {
                var parent = FindParentIssueFor(issues, subTask);
                subTask.Additions.IncludedInSprint = parent?.Additions.IncludedInSprint;
            }
        }

        private IssueModel FindParentIssueFor(IList<IssueModel> issues, IssueModel subTask)
        {
            if (subTask.Fields?.Parent == null)
            {
                return null;
            }

            return issues.FirstOrDefault(t => t.Id == subTask.Fields.Parent.Id);
        }

        public void Save<TModel>(TModel model) where TModel : class, IHasIdentifier
        {
            Guard.IsNotNull(model, nameof(model));

            CacheStorage.Put(model);

            if (model is IHasAdditions modelWithAdditions)
            {
                Save(modelWithAdditions.GetAdditions());
            }
        }

        public void Save<TModel>(TModel model, CacheKeyInfo cacheKeyInfo) where TModel : class, IHasIdentifier
        {
            Guard.IsNotNull(model, nameof(model));

            CacheStorage.Put(cacheKeyInfo.Key, model);

            if (model is IHasAdditions modelWithAdditions)
            {
                Save(modelWithAdditions.GetAdditions());
            }
        }

        private bool HasChangesInJira(IssueModel issue, IDictionary<string, DateTime> updateDates)
        {
            var hasNewUpdates = (updateDates.ContainsKey(issue.Id) && (updateDates[issue.Id] != issue.Fields.Updated));

            return hasNewUpdates;
        }

        private void UpdateTransitionDates(IssueModel issue, PagedChangelogModel changelog)
        {
            Guard.IsNotNull(issue.Additions, nameof(issue.Additions));
            var issueAdditions = issue.Additions;

            issueAdditions.IncludedInSprint = FindFirstHistoryItem(changelog.Histories, issue.Sprint) ??
                new IssueTransitionModel()
                {
                    Id = "0",
                    Author = issue.Fields.Reporter,
                    Created = issue.Fields.Created,
                    StatusName = issue.Fields.Status.Name
                };

            issueAdditions.TechnicalAnalysisCompleted = FindFirstHistoryItem(
                changelog.Histories,
                JiraIssueStatus.DevInProgress,
                JiraIssueStatus.WaitForDev,
                JiraIssueStatus.PlannedInSprint,
                JiraIssueStatus.Closed);

            issueAdditions.DevelopmentCompleted = FindFirstHistoryItem(
                changelog.Histories,
                JiraIssueStatus.CodeReview,
                JiraIssueStatus.Closed);

            issueAdditions.СodeReviewCompleted = FindFirstHistoryItem(
                changelog.Histories,
                JiraIssueStatus.WaitForTests, 
                JiraIssueStatus.TestInPorgress,
                JiraIssueStatus.Completed,
                JiraIssueStatus.Closed);

            if (issue.IsTestingSupported)
            {
                issueAdditions.TestingCompleted = FindFirstHistoryItem(
                    changelog.Histories,
                    JiraIssueStatus.RegressionIssues,
                    JiraIssueStatus.IntegrationTests,
                    JiraIssueStatus.WaitForRelease,
                    JiraIssueStatus.WaitForUserAccept,
                    JiraIssueStatus.WaitForMerge,
                    JiraIssueStatus.Completed,
                    JiraIssueStatus.Closed);
            }
            
            Logger.Info($"Issue {issue.Key} updated.");
        }

        private IssueTransitionModel FindFirstHistoryItem(List<IssueHistoryModel> histories, params String[] statuses)
        {
            return histories
                .Where(p => HasIssueStatus(p, statuses))
                .OrderBy(p => p.Created)
                .ProjectTo<IssueTransitionModel>()
                .FirstOrDefault();
        }

        private IssueTransitionModel FindFirstHistoryItem(List<IssueHistoryModel> histories, SprintModel sprint)
        {
            return histories
                .Where(p => HasIssueSprint(p, sprint))
                .OrderBy(p => p.Created)
                .ProjectTo<IssueTransitionModel>()
                .FirstOrDefault();
        }

        private bool HasIssueStatus(IssueHistoryModel model, params String[] statuses)
        {
            return model.Items
                .Any(p => p.FieldId == "status" 
                    &&  p.FieldType == "jira" 
                    &&  statuses.Any(s => s.Equals(p.ToValue, StringComparison.InvariantCultureIgnoreCase)));
        }

        private bool HasIssueSprint(IssueHistoryModel model, SprintModel sprint)
        {
            return model.Items.Any(p => p.Field == "Sprint" && p.FieldType == "custom" && p.ToValue == sprint.Name);
        }
    }
}
