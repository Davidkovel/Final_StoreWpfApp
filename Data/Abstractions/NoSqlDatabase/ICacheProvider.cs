namespace Data.Abstractions.NoSqlDatabase;

public interface ICacheProvider
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}