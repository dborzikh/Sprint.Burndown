using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/calendar")]
    public class CalendarController : Controller
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("holidays")]
        public ActionResult<object> GetHolidays()
        {
            var holidays = _calendarService.GetHolidays(DateTime.Today);

            return new
            {
                Holidays = holidays
            };
        }
    }
}