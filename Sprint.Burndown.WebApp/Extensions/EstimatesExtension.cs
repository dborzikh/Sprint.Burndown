using System;
using System.Collections.Generic;
using System.Linq;

using Sprint.Burndown.WebApp.Utils;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class EstimatesExtension
    {
        public const int SECONDS_IN_HOUR = 3600;

        public const int SECONDS_IN_WORKDAY = 8 * SECONDS_IN_HOUR;

        public static string ToStringEstimate(this int estimateSeconds)
        {
            var hours = (estimateSeconds / SECONDS_IN_HOUR);
            var minutes = (estimateSeconds - hours * SECONDS_IN_HOUR) / 60;

            return $"{hours}h" + (minutes > 0 ? $" {minutes}m" : "");
        }

        public static DateTime LimitBySprintStart(this DateTime dateTime, DateTime sprintBeginDate)
        {
            return dateTime < sprintBeginDate ? sprintBeginDate : dateTime;
        }

        public static DateTime Nearest(this ISet<DateTime> setOfDates, DateTime date)
        {
            var nearestDate = setOfDates.Where(d => d <= date).Max();

            return nearestDate;
        }

        public static T Nearest<T>(this IEnumerable<T> query, Func<T, DateTime> selector, DateTime date) 
        {
            Guard.IsNotNull(query, nameof(query));
            Guard.IsNotNull(selector, nameof(selector));

            var nearestElement = query
                .Where(p => selector(p) <= date)
                .OrderByDescending(selector)
                .FirstOrDefault();

            return nearestElement;
        }

    }
}
