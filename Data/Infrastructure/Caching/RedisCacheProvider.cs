// using Data.Abstractions.NoSqlDatabase;
//
// namespace Data.Infrastructure.Caching;
//
// public class RedisCacheProvider : ICacheProvider
// {
//     private readonly string _connection;
//     private readonly JsonSerializerSettings _serializerSettings;
//     private ICacheProvider _cacheProviderImplementation;
//
//     public RedisCacheProvider(string connection)
//     {
//         _connection = connection;
//         _serializerSettings = new JsonSerializerSettings
//         {
//             TypeNameHandling = TypeNameHandling.All
//         };
//     }
//
//     public async Task<T> GetAsync<T>(string key)
//     {
//         string cachedJson = await _redisDb.StringGetAsync(cacheKey);
//
//         // 2. Если ничего нет - возвращаем "пусто" (default для типа T)
//         if (string.IsNullOrEmpty(cachedJson))
//             return default;
//
//         // 3. Преобразуем JSON обратно в объект (например, в Product)
//         return JsonConvert.DeserializeObject<T>(cachedJson);
//     }
//
//     public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
//     {
//         // 1. Преобразуем объект (например Product) в JSON-строку
//         string json = JsonConvert.SerializeObject(data);
//
//         // 2. Сохраняем в Redis
//         await _redisDb.StringSetAsync(cacheKey, json, cacheTime);
//     }
//
//     public Task RemoveAsync(string key)
//     {
//         return _cacheProviderImplementation.RemoveAsync(key);
//     }
//
//     public Task<bool> ExistsAsync(string key)
//     {
//         return _cacheProviderImplementation.ExistsAsync(key);
//     }
//
//     // ... остальные методы
// }