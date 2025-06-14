using System.Data;
using Data.Database.Abstractions;
using Microsoft.Data.SqlClient;

namespace Data.Database.Providers;

public class SqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;
    private readonly Task _initializationTask;

    public SqlConnectionFactory(string connectionString, Task initializationTask)
    {
        _connectionString = connectionString;
        _initializationTask = initializationTask;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        await _initializationTask;
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}