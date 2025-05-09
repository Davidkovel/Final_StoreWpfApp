using Core.Entity;

namespace Core.Repository;

public abstract class ProductRepository
{
    public abstract Task<IEnumerable<Product>> GetProductsAsync();
    public abstract Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
    public abstract Task<Product> GetProductByIdAsync(int id);
    public abstract Task AddProductAsync(Product product);
    public abstract Task UpdateProductAsync(Product product);
    public abstract Task DeleteProductAsync(int id);
}