using System;
using System.Collections.Generic;

using Sprint.Burndown.WebApp.JiraModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IJiraService
    {
        Session Login(string username, string password);

        UserModel GetUserInformation(string username, string password);

        bool Logout(Session session);

        IPagingList<BoardModel> GetBoardsInternal(Session session);

        BoardModel GetBoardInternal(Session session, string boardId);

        IPagingList<SprintModel> GetSprintsInternal(Session session, string boardId);

        SprintModel GetSprintInternal(Session session, string sprintId);

        IPagingList<IssueModel> GetIssuesInternal(Session session, string sprintId);

        IssueChangelogModel GetIssueChangelogInternal(Session session, string issueId);

        IssueWorklogModel GetIssueWorklogInternal(Session session, string issueId);

        IDictionary<string, DateTime> GetIssueUpdatesInternal(Session session, string sprintId);
    }
}
