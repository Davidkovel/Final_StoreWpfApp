using Core.Entity;
using Data.Abstractions.NoSqlDatabase;

namespace DekstopApp.Services;

public class ProductSearchService
{
    private readonly IProductSearchProvider _productSearchProvider;

    public ProductSearchService(IProductSearchProvider productSearchProvider)
    {
        _productSearchProvider = productSearchProvider;
    }

    public async Task InitializeIndex()
    {
        await _productSearchProvider.CreateIndexAsync("products");
    }

    public async Task AddProduct(Product product)
    {
        await _productSearchProvider.IndexProductAsync(product);
    }

    public async Task<List<Product>> Search(string query)
    {
        return await _productSearchProvider.SearchProductsAsync(query);
    }
}