using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IIssueService
    {
        IList<IssueViewModel> GetIssues(SprintViewModel sprint);

        IList<IssueViewModel> GetUpdatedIssues(SprintViewModel sprint);
    }
}
