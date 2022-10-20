using Newtonsoft.Json;

namespace Sprint.Burndown.WebApp.JiraModels
{
    public class IssueHistoryItemModel
    {
        public string Field { get; set; }

        public string FieldType { get; set; }

        public string FieldId { get; set; }

        [JsonProperty("fromString")]
        public string FromValue { get; set; }

        [JsonProperty("toString")]
        public string ToValue { get; set; }
    }
}
