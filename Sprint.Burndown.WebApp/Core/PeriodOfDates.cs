using System;
using System.Collections;
using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.Core
{
    public class PeriodOfDates : IEnumerable<DateTime>
    {
        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public static PeriodOfDates Create(DateTime beginDate, DateTime endDate)
        {
            return new PeriodOfDates
            {
                BeginDate = beginDate,
                EndDate = endDate
            };
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            for (var date = BeginDate; date <= EndDate; date = date.AddDays(1))
            {
                yield return date;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
