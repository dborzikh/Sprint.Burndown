using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Models;

namespace Sprint.Burndown.WebApp.ViewModels
{
    public class UserPreferencesViewModel : IHasIdentifier
    {
        public string Id { get; set; }

        public int DefaultBoardId { get; set; }

        public SubViewType[] PreferredSubViews { get; set; }
    }
}
