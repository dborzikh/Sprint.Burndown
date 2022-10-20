using System;
using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class SeriesDataViewModel
    {
        public int Index { get; set; }

        public DateTime FactDate { get; set; }

        public int WorkHours { get; set; }

        public int WorkDone { get; set; }

        public IEnumerable<SeriesTaskViewModel> Tasks { get; set; }
    }
}
