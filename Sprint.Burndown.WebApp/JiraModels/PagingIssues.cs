using System.Collections.Generic;

using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class PagingIssues<T> : PagingList<T>, IPagingList<T>
    {
        public List<T> Issues
        {
            get => Values;
            set
            {
                Values = value;
            }
        }
    }
}
