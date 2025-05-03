using System.Data;
using Dapper;
using Data.DBCommands;
using Microsoft.Data.SqlClient;

namespace Data.DBProvider;

public interface IDatabaseProvider
{
    Task InitializeDatabaseAsync();
    Task ResetDatabaseAsync();
    Task<IDbConnection> CreateConnectionAsync();
}

public class SqlServerDatabaseProvider : IDatabaseProvider
{
    private readonly string _connectionString;

    public SqlServerDatabaseProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            // @Todo: Разобрать с ошибкой  Database initialization failed: Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'startIndex')

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Создаем БД если не существует
            await ExecuteNonQueryAsync(connection, DbCommands.CreateDbCommandWithNotExists("Shop"));

            // Используем нашу БД
            // await ExecuteNonQueryAsync(connection, DbCommands.UseDbCommand("Shop"));

            // Создаем таблицы
            await ExecuteNonQueryAsync(connection, DbCommands.CreateCategoriesTableIfNotExists());
            await ExecuteNonQueryAsync(connection, DbCommands.CreateTablesCommandIfNotExist());
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization failed: {ex.Message}");
            throw;
        }
    }

    public async Task ResetDatabaseAsync()
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Удаляем таблицы
            await ExecuteNonQueryAsync(connection, DbCommands.DropTablesCommand());

            // Создаем заново
            await InitializeDatabaseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database reset failed: {ex.Message}");
            throw;
        }
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    private async Task ExecuteNonQueryAsync(SqlConnection connection, string commandText)
    {
        try
        {
            await connection.ExecuteAsync(commandText);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command: {commandText}\nError: {ex.Message}");
            throw;
        }
    }
}