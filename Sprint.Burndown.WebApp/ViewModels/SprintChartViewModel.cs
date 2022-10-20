using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class SprintChartViewModel
    {
        public string SprintName { get; set; }

        public DateTime PlanBeginDate { get; set; }

        public DateTime PlanEndDate { get; set; }

        public int PlanBeginIndex { get; set; }

        public int PlanEndIndex { get; set; }

        public int PlanTotalEstimate { get; set; }

        public DateTime FactBeginDate { get; set; }

        public DateTime FactEndDate { get; set; }

        public int FactBeginIndex { get; set; }

        public int FactEndIndex { get; set; }

        public decimal TotalEstimate { get; set; }

        public IEnumerable<String> Categories { get; set; }

        public IEnumerable<SeriesDataViewModel> Data { get; set; }
    }
}
