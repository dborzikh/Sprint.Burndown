using System.Collections.Generic;
using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class PagingList<T> : IPagingList<T> 
    {
        public int? Total { get; set; }

        public int MaxResults { get; set; }

        public int StartAt { get; set; }

        public bool? IsLast { get; set; }

        public List<T> Values { get; set; } = new List<T>();
        
        public bool HasIncompleteResuls()
        {
            if (IsLast.HasValue)
            {
                return !IsLast.Value;
            }
            else if (Total.HasValue)
            {
                return Total.Value > Values.Count;
            }

            return false;
        }
    }
}
