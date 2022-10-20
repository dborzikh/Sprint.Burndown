using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Hubs;

namespace Sprint.Burndown.WebApp.Services
{
    public class NotificationService : INotificationService
    {
        private int _totalItems = 0;

        private string _notificationKey = null;

        private IHubContext<LoadingNotificationHub> HubContext { get; set; }

        public NotificationService(IHubContext<LoadingNotificationHub> hubContext)
        {
            HubContext = hubContext;
        }

        public async Task LoadingStarted(int totalItems, string key)
        {
            _totalItems = totalItems;
            _notificationKey = key;

            await HubContext.Clients.All.SendAsync("started", totalItems, _notificationKey);
        }

        public async Task LoadingProcessed(int currentItem)
        {
            await HubContext.Clients.All.SendAsync("processed", currentItem, _totalItems, _notificationKey);
        }

        public async Task LoadingFinished()
        {
            await HubContext.Clients.All.SendAsync("finished", _notificationKey);
        }
    }
}
