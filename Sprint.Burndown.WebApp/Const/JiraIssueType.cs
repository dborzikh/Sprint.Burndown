using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprint.Burndown.WebApp.Const
{
    public class JiraIssueType
    {
        public static readonly String[] QaTasks = 
        {
            QaTask,
            QaSubTask
        };

        public static readonly String[] DevSubTasks =
        {
            SubTask,
            TechnicalSubTask
        };

        public const String QaTask = "QA Task";

        public const String QaSubTask = "QA Sub-task";

        public const String SubTask = "Sub-task";

        public const String TechnicalSubTask = "Technical Sub-task";
    }
}
