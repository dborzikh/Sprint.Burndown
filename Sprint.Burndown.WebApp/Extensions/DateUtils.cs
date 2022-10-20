using System;

namespace Sprint.Burndown.WebApp.Extensions
{
    public class DateUtils
    {
        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }
    }
}
