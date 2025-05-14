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

    public override async Task AddItemToCartAsync(Cart cart)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", cart.ProductId);
        parameters.Add("Quantity", cart.Quantity);
        await connection.ExecuteAsync(_commandProvider.AddItemToCart(), parameters);
    }

    public override async Task DeleteItemFromCartAsync(int productId)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        await connection.ExecuteAsync(_commandProvider.DeleteItemFromCart(), parameters);
    }

    public override async Task ClearCartAsync()
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(_commandProvider.ClearCart());
    }
}