using Core.Entity;
using Data.Abstractions.NoSqlDatabase;

namespace Data.Infrastructure.ElasticSearch;

using Nest;

public class ElasticsearchProvider : IProductSearchProvider
{
    private readonly IElasticClient _client;

    public ElasticsearchProvider(string url = "http://localhost:9200")
    {
        var settings = new ConnectionSettings(new Uri(url))
            .DefaultIndex("products");

        _client = new ElasticClient(settings);
        
    }
    
    // @Todo Исправть проблему с подлкючение так как мб порт занят из за этого Elasticsearch не запускается

    public async Task CreateIndexAsync(string indexName)
    {
        if (!(await _client.Indices.ExistsAsync(indexName)).Exists)
        {
            await _client.Indices.CreateAsync(indexName, c => c
                .Map<Product>(m => m.AutoMap())
            );
        }
    }

    public async Task IndexProductAsync(Product product)
    {
        try 
        {
            var response = await _client.IndexDocumentAsync(product);
            await _client.Indices.RefreshAsync();
            if (!response.IsValid)
            {
                Console.WriteLine($"Ошибка индексации: {response.DebugInformation}");
            }
            else
            {
                Console.WriteLine($"Документ добавлен, ID: {response.Id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        var response = await _client.SearchAsync<Product>(s => s
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(query)
                )
            )
        );

        return response.Documents.ToList();
    }

    public async Task<IEnumerable<Product>> GetSuggestionsAsync(string query, int size, CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<Product>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.Name)
                        .Field(p => p.Description)
                    )
                    .Query(query)
                    .Fuzziness(Fuzziness.Auto)
                )
            )
            .Size(size)
            .Highlight(h => h
                .Fields(f => f
                    .Field(p => p.Name)
                )
            ), ct);

        return response.Documents;
    }
}