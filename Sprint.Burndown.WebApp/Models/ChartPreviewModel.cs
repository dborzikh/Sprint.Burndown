using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;

namespace Sprint.Burndown.WebApp.Models
{
    [CacheEntity("ChartPreview")]
    public class ChartPreviewModel : IHasIdentifier
    {
        public string Id
        {
            get => SprintId;
            set => SprintId = value;
        }

        public string SprintId { get; set; }

        public string ImageBody { get; set; }
    }
}
