using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class BoardViewModel : IHasIdentifier
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
