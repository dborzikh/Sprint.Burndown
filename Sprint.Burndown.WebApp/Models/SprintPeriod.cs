using System;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.Core;

namespace Sprint.Burndown.WebApp.Models
{
    public class SprintPeriod : PeriodOfDates
    {
        public string SprintId { get; set; }

        public int Velocity { get; set; }

        public static SprintPeriod Create(string sprintId, DateTime beginDate, DateTime endDate, int velocity = 220)
        {
            return new SprintPeriod
            {
                SprintId = sprintId,
                BeginDate = beginDate,
                EndDate = endDate,
                Velocity = velocity
            };
        }
    }
}
