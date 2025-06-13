using System.Data;

namespace Data.Database.Abstractions;

public interface IDatabaseProvider
{
    Task InitializeDatabaseAsync();
    Task ResetDatabaseAsync();
    Task<IDbConnection> CreateConnectionAsync();
}