using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NLog;

namespace Sprint.Burndown.WebApp.Const
{
    public class LoggerType
    {
        public static ILogger General => LogManager.GetLogger("general");

        public static ILogger System => LogManager.GetLogger("system");

        public static ILogger Integration => LogManager.GetLogger("integration");
    }
}
