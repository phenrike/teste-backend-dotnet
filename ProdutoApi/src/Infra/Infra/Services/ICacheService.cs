using Domain.Entities;

namespace Infra.Services;

public interface ICacheService
{
    Task<T> GetOrCreateCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFunction, TimeSpan expiration);
    Task<(List<Produto>, int)> GetOrCreateCacheListAsync(
            string cacheKey,
            Func<Task<(List<Produto>, int)>> factory,
            TimeSpan expiration);
}