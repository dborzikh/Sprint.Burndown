using System;

using Sprint.Burndown.WebApp.Models;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class UpdateSprintDatesViewModel
    {
        public string SprintId { get; set; }

        public PeriodType PeriodType { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
