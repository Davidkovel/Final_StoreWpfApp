using Data.Abstractions.Replication;
using Microsoft.Data.SqlClient;

namespace Data.Infrastructure.Replication;

// Data.Infrastructure/Replication/
public class SqlReplicationConfigurator : IReplicationConfigurator
{
    private readonly string _connectionString;
    private readonly string _publicationName = "MainPublication";
    private IReplicationConfigurator _replicationConfiguratorImplementation;

    public SqlReplicationConfigurator(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ConfigurePublisherAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            EXEC sp_addpublication 
                @publication = @pubName,
                @sync_method = 'native',
                @repl_freq = 'continuous',
                @independent_agent = 'true';
            
            EXEC sp_addpublication_snapshot 
                @publication = @pubName;", connection);

        cmd.Parameters.AddWithValue("@pubName", _publicationName);
        await connection.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ConfigureSubscriberAsync(string publisherConnectionString)
    {
        await using var connection = new SqlConnection(_connectionString);
        var cmd = new SqlCommand(@"
            EXEC sp_addsubscription 
                @publication = @pubName,
                @subscriber = @publisher,
                @destination_db = @dbName,
                @subscription_type = 'Push';", connection);

        cmd.Parameters.AddWithValue("@pubName", _publicationName);
        cmd.Parameters.AddWithValue("@publisher", publisherConnectionString);
        cmd.Parameters.AddWithValue("@dbName", new SqlConnectionStringBuilder(_connectionString).InitialCatalog);

        await connection.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public Task AddArticleToPublicationAsync(string tableName)
    {
        return _replicationConfiguratorImplementation.AddArticleToPublicationAsync(tableName);
    }
}