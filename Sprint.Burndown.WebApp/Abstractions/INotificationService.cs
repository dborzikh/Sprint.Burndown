using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface INotificationService
    {
        Task LoadingStarted(int totalItems, string key);

        Task LoadingProcessed(int currentItem);

        Task LoadingFinished();
    }
}
