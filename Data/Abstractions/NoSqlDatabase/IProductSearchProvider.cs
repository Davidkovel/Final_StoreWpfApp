using Core.Entity;

namespace Data.Abstractions.NoSqlDatabase;

public interface IProductSearchProvider
{
    Task CreateIndexAsync(string indexName);
    Task IndexProductAsync(Product product);
    Task<List<Product>> SearchProductsAsync(string query);
}