using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.Core
{
    public class GlobalContext : IGlobalContext
    {
        private bool _hasIncompleteData;

        public bool HasIncompleteData
        {
            get => _hasIncompleteData;
            set => _hasIncompleteData = _hasIncompleteData || value;
        }

        public string AuthenticationToken { get; set; }
    }
}
