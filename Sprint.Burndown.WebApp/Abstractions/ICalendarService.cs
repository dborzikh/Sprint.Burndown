using System;
using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface ICalendarService
    {
        bool IsWorkDay(DateTime date);

        HashSet<DateTime> GetHolidays(DateTime baseDate);

        HashSet<DateTime> GetHolidays();

        DateTime GetNearestWorkDay(DateTime date);
    }
}
