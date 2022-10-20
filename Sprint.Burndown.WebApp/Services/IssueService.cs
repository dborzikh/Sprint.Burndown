using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Services
{
    public class IssueService : IIssueService
    {
        private IRepository Repository { get; }

        public IssueService(IRepository repository)
        {
            Repository = repository;
        }

        public IList<IssueViewModel> GetUpdatesIssues(SprintViewModel sprint)
        {
            return Repository
                .GetIssues(sprint.Id)
                .ProjectTo<IssueViewModel>()
                .ToList();
        }

        public IList<IssueViewModel> GetIssues(SprintViewModel sprint)
        {
            var issues = Repository
                .GetIssues(sprint.Id)
                .ProjectTo<IssueViewModel>()
                .ToList();

            return issues;
        }

        public IList<IssueViewModel> GetUpdatedIssues(SprintViewModel sprint)
        {
            return Repository
                .GetUpdatedIssues(sprint.Id)
                .ProjectTo<IssueViewModel>()
                .ToList();
        }

    }
}
