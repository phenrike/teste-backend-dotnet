using Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace Infra.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _cache = connectionMultiplexer.GetDatabase();
    }

    public async Task<T> GetOrCreateCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFunction, TimeSpan expiration)
    {
        var cachedData = await _cache.StringGetAsync(cacheKey);
        if (!cachedData.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        var result = await fetchFunction();
        var serializedResult = JsonSerializer.Serialize(result);

        await _cache.StringSetAsync(cacheKey, serializedResult, expiration);

        return result;
    }

    public async Task<(List<Produto>, int)> GetOrCreateCacheListAsync(
            string cacheKey,
            Func<Task<(List<Produto>, int)>> factory,
            TimeSpan expiration)
    {
        var cachedData = await _cache.StringGetAsync(cacheKey);
        if (!cachedData.IsNullOrEmpty)
        {
            var produtoResult = JsonSerializer.Deserialize<ProdutoResult>(cachedData);
            return (produtoResult.Items, produtoResult.TotalItems);
        }

        var result = await factory();
        var serializedResult = JsonSerializer.Serialize(new ProdutoResult
        {
            Items = result.Item1,
            TotalItems = result.Item2
        });

        await _cache.StringSetAsync(cacheKey, serializedResult, expiration);

        return result;
    }
}