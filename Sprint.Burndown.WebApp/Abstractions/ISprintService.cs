using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Sprint.Burndown.WebApp.Models;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface ISprintService
    {
        IList<SprintViewModel> GetSprints(BoardViewModel board);

        SprintReportViewModel GetSprintReport(SprintViewModel sprint);

        SprintViewModel ToggleFavorite(SprintViewModel sprint);

        void SaveChartPreview(ImageViewModel chartImage);

        ChartPreviewModel GetChartPreview(string sprintId);

        void UpdateSprintDates(UpdateSprintDatesViewModel sprintDates);
    }
}
