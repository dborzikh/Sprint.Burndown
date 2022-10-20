using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sprint.Burndown.WebApp.ExternalServices;
using Sprint.Burndown.WebApp.Models;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IProductionScheduleService
    {
        Lazy<IList<HolidayModel>> GetSchedule();
    }
}
