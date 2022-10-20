using System.Collections.Generic;

namespace Sprint.Burndown.WebApp.Abstractions
{
    public interface ICacheStorage
    {
        T Get<T>(string itemId) where T : class, IHasIdentifier;

        IList<T> Get<T>(IEnumerable<string> itemIds) where T: class, IHasIdentifier;

        T Get<T>(string cacheKey, string itemId) where T : class, IHasIdentifier;

        IList<T> GetAll<T>() where T : class, IHasIdentifier;

        IList<T> GetAll<T>(string cacheKey) where T : class, IHasIdentifier;

        void Put<T>(T model) where T : class, IHasIdentifier;

        void Put<T>(string cacheKey, T model) where T : class, IHasIdentifier;

        void Put<T>(string cacheKey, IEnumerable<T> models) where T : class, IHasIdentifier;

        bool Exists(string key);
    }
}
