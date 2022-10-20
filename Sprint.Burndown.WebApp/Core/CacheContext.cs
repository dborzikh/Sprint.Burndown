namespace Sprint.Burndown.WebApp.Core
{
    public class CacheContext
    {
        public bool InvalidateCache { get; set; }

        public bool AllowPartialUpdate { get; set; }

        public bool UseCachedValues => !InvalidateCache;

        public static CacheContext Default => new CacheContext { InvalidateCache = false, AllowPartialUpdate = false};
    }
}
