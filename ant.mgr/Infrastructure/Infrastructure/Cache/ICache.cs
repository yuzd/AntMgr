using System;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public interface ICache
    {
        bool Set<T>(string cacheKey, T cacheValue);
        Task<bool> SetAsync<T>(string cacheKey, T cacheValue);
        bool Set<T>(string cacheKey, T cacheValue, TimeSpan expiration);
        Task<bool> SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration);

        T Get<T>(string cacheKey);
        Task<T> GetAsync<T>(string cacheKey);
        T Get<T>(string cacheKey, Func<(T, TimeSpan)> dataRetriever);
        Task<T> GetAsync<T>(string cacheKey, Func<(T, TimeSpan)> dataRetriever);

        bool Remove(string cacheKey);
        Task<bool> RemoveAsync(string key);
        bool Exists(string cacheKey);
    }
}
