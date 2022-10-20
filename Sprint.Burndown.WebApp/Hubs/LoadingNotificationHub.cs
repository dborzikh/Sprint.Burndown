using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.Hubs
{
    public class LoadingNotificationHub : Hub
    {
        private int _totalItems = 0;

        public async Task LoadingStarted(int totalItems, int sprintId)
        {
            _totalItems = totalItems;
            await Clients.All.SendAsync("started", totalItems, sprintId);
        }

        public async Task LoadingProcessed(int currentItem, int sprintId)
        {
            await Clients.All.SendAsync("processed", currentItem, _totalItems, sprintId);
        }

        public async Task LoadingFinished(int sprintId)
        {
            await Clients.All.SendAsync("finished", sprintId);
        }
    }
}
