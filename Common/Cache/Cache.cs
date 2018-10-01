using RulesService.Exceptions;
using System;
using System.Runtime.Caching;

namespace RulesService.Caching
{
    public class Cache<T> : ICache<T>
    {
        private static ObjectCache cache;
        private readonly CacheItemPolicy cachePolicy;

        public Cache()
        {
            cache = MemoryCache.Default;
            this.cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
            };
        }

        public object GetCachedItem(
            string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException("cacheKey");
            }

            try
            {
                return cache[cacheKey];
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to get item {cacheKey} from cache.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }

        public void Add(
            string cacheKey, 
            T cacheItem)
        {

            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException("cacheKey");
            }

            if (cacheItem == null)
            {
                throw new ArgumentNullException("cacheItem");
            }

            try
            {
                cache.Set(cacheKey, cacheItem, this.cachePolicy);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to add item {cacheKey} to cache.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }

        public void Clear(
            string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException("cacheKey");
            }

            try
            {
                cache.Remove(cacheKey);
            }
            catch (Exception ex)
            {
                // Throw service exception.
                string customErrorMessage = $"Unable to remove item {cacheKey} from cache.";
                throw new ServiceException(customErrorMessage, ex);
            }
        }
    }
}
