using System;
using System.Collections.Generic;

using Sprint.Burndown.WebApp.JiraModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IJiraFacade
    {
        Session Login(string username, string password);

        bool Logout(Session session);

        UserModel GetUserInformation(string username, string password);

        Lazy<IPagingList<BoardModel>> GetBoards(Lazy<Session> session);

        Lazy<BoardModel> GetBoard(Lazy<Session> session, string boardId);

        Lazy<IPagingList<SprintModel>> GetSprints(Lazy<Session> session, string boardId);

        Lazy<SprintModel> GetSprint(Lazy<Session> session, string sprintId);

        Lazy<IPagingList<IssueModel>> GetIssues(Lazy<Session> session, string sprintId);

        Lazy<IssueChangelogModel> GetIssueChangelog(Lazy<Session> session, string issueId);

        IDictionary<string, DateTime> GetIssueUpdates(Lazy<Session> session, string sprintId);
    }
}
