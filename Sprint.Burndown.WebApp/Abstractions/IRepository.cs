using System.Collections.Generic;

using Sprint.Burndown.WebApp.JiraModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IRepository
    {
        IEnumerable<BoardModel> GetBoards();

        BoardModel GetBoardById(string boardId);

        IEnumerable<SprintModel> GetSprints(string boardId);

        SprintModel GetSprintById(string sprintId);

        IEnumerable<IssueModel> GetIssues(string sprintId);

        IEnumerable<IssueModel> GetUpdatedIssues(string sprintId);

        void Save<TModel>(TModel model) where TModel : class, IHasIdentifier;
    }
}
