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
        var testProduct = new Product("Test Product",
            "This is a sample product description.",
            19.99m,
            "C:\\Users\\David\\RiderProjects\\StoreWpfApp\\Data\\Persistence\\Images\\Без названия (1).jpg",
            1,
            100);

        await AddProduct(testProduct);
        // Console.WriteLine("Index initialized successfully");
    }

    public async Task AddProduct(Product product)
    {
        try
        {

            Console.WriteLine($"Adding product {product.Name} to ElasticSearch");
            await _productSearchProvider.IndexProductAsync(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product to ElasticSearch: {ex.Message}");
        }
    }

    public async Task<List<Product>> Search(string query)
    {
        Console.WriteLine($"Searching for {query}");
        Console.WriteLine("Showing all products in ElasticSearch:");
        await ShowAllProductsElasticSearch();
        return await _productSearchProvider.SearchProductsAsync(query);
    }

    public async Task<IEnumerable<Product>> GetSuggestions(string query, int size, CancellationToken ct = default)
    {
        return await _productSearchProvider.GetSuggestionsAsync(query, size, ct);
    }

    private async Task<List<Product>> ShowAllProductsElasticSearch()
    {
        var products = await _productSearchProvider.SearchProductsAsync("*");
        if (products.Any())
        {
            Console.WriteLine("[DEBUG] Found Products:");
            foreach (var p in products)
            {
                Console.WriteLine($"- {p.Name} | {p.Description} | {p.Price}₽");
            }
        }
        else
        {
            Console.WriteLine("No products found.");
        }

        return products;
    }
}