using System;

namespace Sprint.Burndown.WebApp.Core
{
    [Flags]
    public enum CacheOptions
    {
        Cached = 0x01,

        Invalidated = 0x02,

        CachedPartially = 0x04
    }
}
