using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Attributes;
using Sprint.Burndown.WebApp.JiraModels;
using Sprint.Burndown.WebApp.Utils;

using StackExchange.Redis;

namespace Sprint.Burndown.WebApp.Services
{
    public class RedisStorage : ICacheStorage, IDisposable
    {
        private const string RedisCachePrefix = "Sprint.Burndown";
        
        private const string DefaultRedisHost = "localhost";

        private ConnectionMultiplexer Connection { get; set;  }

        private IDatabase Database { get; }

        public RedisStorage()
        {
            try
            {
                Connection = ConnectionMultiplexer.Connect(DefaultRedisHost);
                Database = Connection.GetDatabase();
            }
            catch
            { 
            }
        }

        public T Get<T>(string itemId) where T : class, IHasIdentifier
        {
            var cacheKey = GetCacheKeyFor<T>();
            var value = Retry.Do(() => Database.HashGet(cacheKey, itemId));

            return value.HasValue 
                ? JsonConvert.DeserializeObject<T>(value)
                : null;
        }

        public T Get<T>(string cacheKey, string itemId) where T : class, IHasIdentifier
        {
            cacheKey = GetCacheKeyWithPrefix(cacheKey);
            var value = Retry.Do(() => Database.HashGet(cacheKey, itemId));

            return value.HasValue
                ? JsonConvert.DeserializeObject<T>(value)
                : null;
        }

        public IList<T> Get<T>(IEnumerable<string> itemIds) where T : class, IHasIdentifier
        {
            Guard.IsNotNull(itemIds, nameof(itemIds));

            var cacheKey = GetCacheKeyFor<T>();
            var entityKeys = itemIds
                .Select(p => (RedisValue)p.ToString())
                .ToArray();

            var values = Retry.Do(() => Database.HashGet(cacheKey, entityKeys));

            var result = values
                .Where(v => v.HasValue)
                .Select(v => JsonConvert.DeserializeObject<T>(v))
                .ToList();

            return result;
        }

        public IList<T> GetAll<T>() where T : class, IHasIdentifier
        {
            var cacheKey = GetCacheKeyFor<T>();
            var entries = Retry.Do(() => Database.HashGetAll(cacheKey));

            var result = entries
                .Where(entry => entry.Value.HasValue)
                .Select(entry => JsonConvert.DeserializeObject<T>(entry.Value))
                .ToList();

            return result;
        }

        public IList<T> GetAll<T>(string cacheKey) where T : class, IHasIdentifier
        {
            cacheKey = GetCacheKeyWithPrefix(cacheKey);
            var entries = Retry.Do(() => Database.HashGetAll(cacheKey));

            var result = entries
                .Where(entry => entry.Value.HasValue)
                .Select(entry => JsonConvert.DeserializeObject<T>(entry.Value))
                .ToList();

            return result;
        }

        public void Put<T>(T model) where T : class, IHasIdentifier
        {
            Guard.IsNotNull(model, nameof(model));

            var cacheKey = GetCacheKeyFor(model);
            var value = JsonConvert.SerializeObject(model);

            Database.HashSet(cacheKey, model.Id, value);
        }

        public void Put<T>(string cacheKey, T model) where T : class, IHasIdentifier
        {
            Guard.IsNotEmpty(cacheKey, nameof(cacheKey));
            Guard.IsNotNull(model, nameof(model));

            cacheKey = GetCacheKeyWithPrefix(cacheKey);
            var value = JsonConvert.SerializeObject(model);

            Database.HashSet(cacheKey, model.Id, value);
        }

        public void Put<T>(string cacheKey, IEnumerable<T> models) where T : class, IHasIdentifier
        {
            Guard.IsNotEmpty(cacheKey, nameof(cacheKey));
            Guard.IsNotNull(models, nameof(models));

            cacheKey = GetCacheKeyWithPrefix(cacheKey);
            var entityData = models
                .Select(m => new HashEntry(m.Id, JsonConvert.SerializeObject(m)))
                .ToArray();

            Database.KeyDelete(cacheKey);
            Database.HashSet(cacheKey, entityData);
        }

        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        private static readonly IList<Type> SupportedTypes = new List<Type>()
        {
            typeof(SprintAdditions),
            typeof(IssueAdditions),
        };

        private string GetCacheKeyFor<T>() where T: IHasIdentifier
        {
            var cacheSection = typeof(T).GetCustomAttributes(true)
                .OfType<CacheEntityAttribute>()
                .FirstOrDefault();

            Guard.IsNotNull(cacheSection, $"[CacheSection] is not defined for {typeof(T).Name}");

            return GetCacheKeyWithPrefix($"Entities:{cacheSection.Name}");
        }

        private string GetCacheKeyFor(IHasIdentifier model)
        {
            var modelType = model.GetType();
            var cacheSection = modelType
                .GetCustomAttributes(true)
                .OfType<CacheEntityAttribute>()
                .FirstOrDefault();

            Guard.IsNotNull(cacheSection, $"[CacheSection] is not defined for {modelType.Name}");

            return GetCacheKeyWithPrefix($"Entities:{cacheSection.Name}");
        }

        private string GetCacheKeyWithPrefix(string cacheKey)
        {
            Guard.IsNotEmpty(cacheKey, nameof(cacheKey));

            if (cacheKey.StartsWith(RedisCachePrefix))
            {
                return cacheKey;
            }

            return $"{RedisCachePrefix}:{cacheKey}";
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }
    }
}
