using Core.Entity;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Data.DBProvider;

namespace Data.Repository;

public class CartRepositoryImpl : CartRepository
{
    private readonly IDatabaseProvider _databaseProvider;
    private ICartSqlCommandProvider _commandProvider;

    public CartRepositoryImpl(IDatabaseProvider databaseProvider, ICartSqlCommandProvider commandProvider)
    {
        _databaseProvider = databaseProvider;
        _commandProvider = commandProvider;
    }

    public override async Task<IEnumerable<Cart>> GetCartItemsAsync()
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Cart>(_commandProvider.GetCartItems());
    }
}