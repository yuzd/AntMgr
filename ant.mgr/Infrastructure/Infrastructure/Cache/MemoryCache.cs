using Infrastructure.Async;
using Infrastructure.Logging;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public class LocalMemoryCache: ICache
    {
        private static readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        public bool Set<T>(string cacheKey, T cacheValue)
        {
            //var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            try
            {
                cache.Set<T>(cacheKey, cacheValue);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Warn(typeof(LocalMemoryCache).Name, cacheKey , ex);
                return false;
            }
        }

        public async Task<bool> SetAsync<T>(string cacheKey, T cacheValue)
        {
            return await AsyncHelper.GetTask(() => Set<T>(cacheKey, cacheValue), CancellationToken.None);
        }

        public bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            try
            {
                // 这里设置绝对过期时间，避免在频繁访问的时候，缓存不能被更新
                var entryOptions = new MemoryCacheEntryOptions()/*.SetPriority(CacheItemPriority.NeverRemove)*/.SetAbsoluteExpiration(expiration);
                cache.Set<T>(cacheKey, cacheValue, entryOptions);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Warn(typeof(LocalMemoryCache).Name, cacheKey, ex);
                return false;
            }
         
        }

        public async Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            return await AsyncHelper.GetTask(() => Set<T>(cacheKey, cacheValue , expiration), CancellationToken.None);
        }

        public T Get<T>(string cacheKey)
        {
            if (cache.TryGetValue(cacheKey, out T result))
            {
                return result;
            }
            return default(T);
        }

        public async Task<T> GetAsync<T>(string cacheKey)
        {
            return await AsyncHelper.GetTask(() => Get<T>(cacheKey), CancellationToken.None);
        }

        public T Get<T>(string cacheKey, Func<(T,TimeSpan)> dataRetriever)
        {
            if (cache.TryGetValue(cacheKey, out T result))
            {
                return result;
            }

            try
            {
                var re = dataRetriever();
                if (re.Item1 != null)
                {
                    if (re.Item2 != null)
                    {
                        Set(cacheKey, re.Item1, re.Item2);
                    }
                    else
                    {
                        Set(cacheKey, re.Item1);
                    }
                }
                return re.Item1;
            }
            catch (Exception)
            {

            }
           
            return default(T);
        }

        public async Task<T> GetAsync<T>(string cacheKey, Func<(T, TimeSpan)> dataRetriever)
        {
            return await AsyncHelper.GetTask(() => Get<T>(cacheKey, dataRetriever), CancellationToken.None);
        }

        public bool Remove(string cacheKey)
        {
            cache.Remove(cacheKey);
            return true;
        }
        public async Task<bool> RemoveAsync(string key)
        {
            return await AsyncHelper.GetTask(() => Remove(key), CancellationToken.None);
        }
        public bool Exists(string cacheKey)
        {
            return cache.TryGetValue(cacheKey, out _);
        }
    }
}
