using System;

using Microsoft.AspNetCore.Html;

using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class SprintViewModel : IHasIdentifier
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string OriginBoardId { get; set; }

        public string Goal { get; set; }

        public DateTime? ActualStartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public string ActualMonth
        {
            get
            {
                var startDate = ActualStartDate ?? StartDate;
                var endDate = ActualStartDate ?? EndDate;

                if (!startDate.HasValue || !endDate.HasValue)
                {
                    return string.Empty;
                }

                var middleDate = (endDate.Value - startDate.Value).Days / 2;
                return startDate.Value.AddDays(middleDate).ToString("MMM/yy");
            }
        }

        public bool IsFavorite { get; set; }

        public int PercentPassed { get; set; }

        public string ChartPreview { get; set; }
    }
}
