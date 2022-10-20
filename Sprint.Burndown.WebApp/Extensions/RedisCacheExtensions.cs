using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Services;
using Sprint.Burndown.WebApp.Utils;

using NLog;

using Sprint.Burndown.WebApp.Const;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class RedisCacheExtensions
    {
        private static readonly ILogger Log = LoggerType.General;

        private static readonly ICacheStorage CacheStorage = new RedisStorage();

        public static IEnumerable<T> FromCache<T>(this Lazy<IPagingList<T>> query, CacheKeyInfo cacheKeys, out bool hasPendingItems) where T : class, IHasIdentifier, new()
        {
            hasPendingItems = false;

            if (query == null)
            {
                return null;
            }

            var hashData = TryGetHashEntries<T>(cacheKeys);
            if (hashData == null)
            {
                if (query.Value != null)
                {
                    var pagingList = query.Value;
                    hasPendingItems = pagingList.HasIncompleteResuls();

                    hashData = pagingList.Values;
                    CacheStorage.Put<T>(cacheKeys.Key, hashData);
                }
            }

            return hashData;
        }

        public static IEnumerable<T> FromCache<T>(this Lazy<IList<T>> query, CacheKeyInfo cacheKeys) where T : class, IHasIdentifier, new()
        {
            if (query == null)
            {
                return null;
            }

            var hashData = TryGetHashEntries<T>(cacheKeys);
            if (hashData == null )
            {
                if (query.Value != null)
                {
                    hashData = query.Value;
                    CacheStorage.Put<T>(cacheKeys.Key, hashData);
                }
            }

            return hashData ?? new T[0];
        }

        public static T FromCacheById<T>(this Lazy<T> entity, CacheKeyInfo cacheKeys, string entityId) where T : class, IHasIdentifier, new()
        {
            if (entity == null)
            {
                return null;
            }

            var hashValue = TryGetHashEntry<T>(cacheKeys, entityId);
            if (hashValue == null)
            {
                if (entity.Value != null)
                {
                    hashValue = entity.Value;
                    CacheStorage.Put<T>(cacheKeys.Key, hashValue);
                }
            }

            return hashValue;
        }

        private static IList<T> TryGetHashEntries<T>(CacheKeyInfo cacheKeyInfo) where T : class, IHasIdentifier, new()
        {
            try
            {
                var cacheKey = cacheKeyInfo.Key;

                return ÑanUseCache(cacheKey) 
                           ? CacheStorage.GetAll<T>(cacheKey) 
                           : null;
            }
            catch(Exception e)
            {
                Log.Error(e);
                return null; // TODO log errors
            }
        }

        private static T TryGetHashEntry<T>(CacheKeyInfo cacheKeys, string hashItemId) where T : class, IHasIdentifier, new()
        {
            try
            {
                var cacheKey = cacheKeys.Key;
                return ÑanUseCache(cacheKey) 
                           ? CacheStorage.Get<T>(cacheKey, hashItemId) 
                           : null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null; // TODO log errors
            }
        }

        public static bool ÑanUseCache(string cacheKey)
        {
            var cacheContext = GetHttpCacheContext();
            return cacheContext.UseCachedValues && CacheStorage.Exists(cacheKey);
        }

        public static CacheContext GetHttpCacheContext()
        {
            var httpContext = IoC.Resolve<IHttpContextAccessor>().HttpContext;
            var cacheContext = httpContext?.Items[nameof(CacheContext)] as CacheContext;

            return cacheContext ?? CacheContext.Default;
        }
    }
}
