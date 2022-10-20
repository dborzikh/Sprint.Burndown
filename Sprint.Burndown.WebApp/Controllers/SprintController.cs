using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/sprint")]
    public class SprintController : Controller
    {
        private ISprintService SprintService { get; }

        private readonly IIssueService _issueService;

        public SprintController(ISprintService sprintService, IIssueService issueService)
        {
            SprintService = sprintService;
            _issueService = issueService;
        }

        [HttpGet("{id}")]
        [InvalidateCache]
        public ActionResult<SprintReportViewModel> Issues(SprintViewModel model)
        {
            var report = SprintService.GetSprintReport(model);

            return report;
        }

        [HttpGet("{id}/updates")]
        [InvalidateCache]
        public ActionResult<object> GetUpdatedIssues(SprintViewModel model)
        {
            var numberOfIssues = _issueService
                .GetUpdatedIssues(model)
                .Count;

            return new
            {
                model.Id,
                RecentlyUpdatedIssues = numberOfIssues
            };
        }

        [HttpPut("{id}/toggleFavorite")]
        public ActionResult<object> ToggleFavorite([FromBody]SprintViewModel model)
        {
            var sprint = SprintService.ToggleFavorite(model);

            return new
            {
                sprint.Id,
                sprint.IsFavorite
            };
        }

        [HttpPut("{id}/saveChart")]
        public ActionResult<object> SaveChart([FromBody]ImageViewModel model)
        {
            SprintService.SaveChartPreview(model);

            return Ok();
        }

        [HttpPut("{id}/updateDates")]
        public ActionResult<object> UpdateDates([FromBody]UpdateSprintDatesViewModel model)
        {
            SprintService.UpdateSprintDates(model);

            return Ok();
        }
    }
}