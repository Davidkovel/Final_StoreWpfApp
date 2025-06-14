using System.Data;

namespace Data.Database.Abstractions;

public interface IConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}