using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class ImageViewModel : IHasIdentifier
    {
        public string Id { get; set; }

        public string ImageBody { get; set; }
    }
}
