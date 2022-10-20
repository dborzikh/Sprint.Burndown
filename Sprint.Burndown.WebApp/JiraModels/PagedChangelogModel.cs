using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class PagedChangelogModel
    {
        public int StartAt { get; set; }

        public int MaxResults { get; set; }

        public int Total { get; set; }

        public List<IssueHistoryModel> Histories { get; set; }
    }
}
