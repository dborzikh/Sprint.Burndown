using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface IPagingList<T> : IPagingInfo
    {
        List<T> Values { get; set; }

        bool HasIncompleteResuls();
    }
}
