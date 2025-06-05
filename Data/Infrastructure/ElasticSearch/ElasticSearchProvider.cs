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
        await _client.IndexDocumentAsync(product);
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
}