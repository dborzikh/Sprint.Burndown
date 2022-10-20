using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Extensions;

namespace Sprint.Burndown.WebApp.Services
{
    public class CalendarService : ICalendarService
    {
        private ICacheStorage CacheStorage { get; }

        private IProductionScheduleService ProductionScheduleService { get; }

        private HashSet<DateTime> _holidays;

        public ISet<DateTime> Holidays => _holidays ?? (_holidays = GetHolidays());

        public CalendarService(ICacheStorage cacheStorage, IProductionScheduleService productionScheduleService)
        {
            CacheStorage = cacheStorage;
            ProductionScheduleService = productionScheduleService;
        }

        public bool IsWorkDay(DateTime date)
        {
            return !Holidays.Contains(date);
        }

        public HashSet<DateTime> GetHolidays()
        {
            var holidayInfos = ProductionScheduleService.GetSchedule()
                .FromCache(CacheKeys.AllHolidays())
                .ToList();

            var lowerLimit = DateTime.Today.Year - 1;
            var upperLimit = DateTime.Today.Year + 1;
            var holidays = holidayInfos
                .Where(info => info.Year >= lowerLimit && info.Year <= upperLimit)
                .SelectMany(info => info)
                .ToHashSet();

            return holidays;
        }

        public HashSet<DateTime> GetHolidays(DateTime baseDate)
        {
            var holidayInfos = ProductionScheduleService.GetSchedule()
                .FromCache(CacheKeys.AllHolidays())
                .ToList();

            var beginLimit = baseDate.AddYears(-2);
            var endLimit = baseDate.AddYears(2);

            var holidays = holidayInfos
                .SelectMany(info => info)
                .Where(date => date >= beginLimit && date <= endLimit)
                .ToHashSet();

            return holidays;
        }

        public DateTime GetNearestWorkDay(DateTime date)
        {
            var workDay = date;
            var skippedDays = 0;

            while (!IsWorkDay(workDay))
            {
                workDay = workDay.AddDays(1);
                skippedDays += 1;

                if (skippedDays > 30)
                {
                    throw new InvalidOperationException($"Failed to find a nearest work day for [{date}]");
                }
            }

            return workDay;
        }

    }
}
