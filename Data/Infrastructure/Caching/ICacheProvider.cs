namespace Data.Abstractions.NoSqlDatabase;

public interface ICacheProvider
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);

    Task EnqueueAsync<T>(string queueName, T item);
    Task<T> DequeueAsync<T>(string queueName);
}