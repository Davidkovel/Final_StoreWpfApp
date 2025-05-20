using Data.Abstractions.NoSqlDatabase;
using StackExchange.Redis;
using System.Text.Json;

namespace Data.Infrastructure.Caching;

public class RedisCacheProvider : ICacheProvider
{
    private readonly IDatabase _redisDb;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisCacheProvider(string connection)
    {
        var redis = ConnectionMultiplexer.Connect(connection);
        _redisDb = redis.GetDatabase();

        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> GetAsync<T>(string key)
    {
        string cachedJson = await _redisDb.StringGetAsync(key);

        if (string.IsNullOrEmpty(cachedJson))
            return default;

        return JsonSerializer.Deserialize<T>(cachedJson, _serializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        string json = JsonSerializer.Serialize(value, _serializerOptions);
        await _redisDb.StringSetAsync(key, json, expiry);
    }

    public Task RemoveAsync(string key)
    {
        return _redisDb.KeyDeleteAsync(key);
    }

    public Task<bool> ExistsAsync(string key)
    {
        return _redisDb.KeyExistsAsync(key);
    }
}