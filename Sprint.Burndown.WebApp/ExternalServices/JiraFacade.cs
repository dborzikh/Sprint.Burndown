using System;
using System.Collections.Generic;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.JiraModels;

namespace Sprint.Burndown.WebApp.ExternalServices
{
    public class JiraFacade : IJiraFacade
    {
        private IJiraService JiraService { get; }

        public JiraFacade(IJiraService jiraService)
        {
            JiraService = jiraService;
        }

        public Session Login(string username, string password)
        {
            return JiraService.Login(username, password);
        }

        public UserModel GetUserInformation(string username, string password)
        {
            return JiraService.GetUserInformation(username, password);
        }

        public bool Logout(Session session)
        {
            return JiraService.Logout(session);
        }

        public Lazy<IPagingList<BoardModel>> GetBoards(Lazy<Session> session)
        {
            return new Lazy<IPagingList<BoardModel>>(() => JiraService.GetBoardsInternal(session.Value));
        }

        public Lazy<BoardModel> GetBoard(Lazy<Session> session, string boardId)
        {
            return new Lazy<BoardModel>(() => JiraService.GetBoardInternal(session.Value, boardId));
        }

        public Lazy<IPagingList<SprintModel>> GetSprints(Lazy<Session> session, string boardId)
        {
            return new Lazy<IPagingList<SprintModel>>(() => JiraService.GetSprintsInternal(session.Value, boardId));
        }

        public Lazy<SprintModel> GetSprint(Lazy<Session> session, string sprintId)
        {
            return new Lazy<SprintModel>(() => JiraService.GetSprintInternal(session.Value, sprintId));
        }

        public Lazy<IPagingList<IssueModel>> GetIssues(Lazy<Session> session, string sprintId)
        {
            return new Lazy<IPagingList<IssueModel>>(() => JiraService.GetIssuesInternal(session.Value, sprintId));
        }

        public Lazy<IssueChangelogModel> GetIssueChangelog(Lazy<Session> session, string issueId)
        {
            return new Lazy<IssueChangelogModel>(() => JiraService.GetIssueChangelogInternal(session.Value, issueId));
        }

        public Lazy<IssueWorklogModel> GetIssueWorklog(Lazy<Session> session, string issueId)
        {
            return new Lazy<IssueWorklogModel>(() => JiraService.GetIssueWorklogInternal(session.Value, issueId));
        }

        public IDictionary<string, DateTime> GetIssueUpdates(Lazy<Session> session, string sprintId)
        {
            return JiraService.GetIssueUpdatesInternal(session.Value, sprintId);
        }
    }
}
