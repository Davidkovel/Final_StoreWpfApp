using System;
using System.Data;
using System.Threading.Tasks;
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
        InitializeDatabaseAsync();
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            // Создаем БД если не существует
            await ExecuteNonQueryAsync(connection, DatabaseCommandProvider.CreateDbCommandWithNotExists("Shop"));

            // Используем нашу БД
            // await ExecuteNonQueryAsync(connection, DbCommands.UseDbCommand("Shop"));

            // Создаем таблицы
            await ExecuteNonQueryAsync(connection, DatabaseCommandProvider.CreateCategoriesTableIfNotExists());
            await ExecuteNonQueryAsync(connection, DatabaseCommandProvider.CreateTablesCommandIfNotExist());
            await ExecuteNonQueryAsync(connection, DatabaseCommandProvider.CreateCartTableIfNotExists());
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
            await ExecuteNonQueryAsync(connection, DatabaseCommandProvider.DropTablesCommand());

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
            // Console.WriteLine($"Executing SQL command: {commandText}");
            await connection.ExecuteAsync(commandText);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command: {commandText}\nError: {ex.Message}");
            throw;
        }
    }
}