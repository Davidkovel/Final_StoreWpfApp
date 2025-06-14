using System.Data;
using Core.Entity;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Data.Database.Abstractions;
using Data.DBProvider;
using Microsoft.Data.SqlClient;

namespace Data.Repository;

public class CartRepositoryImpl : CartRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private ICartSqlCommandProvider _commandProvider;

    public CartRepositoryImpl(IConnectionFactory connectionFactory, ICartSqlCommandProvider commandProvider)
    {
        _connectionFactory = connectionFactory;
        _commandProvider = commandProvider;
    }

    public override async Task<IEnumerable<Cart>> GetCartItemsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Cart>(_commandProvider.GetCartItems());
    }

    public override async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(string userId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        return await connection.QueryAsync<Cart>(_commandProvider.GetCartItemByUserId(), parameters);
    }

    public override async Task AddItemToCartAsync(Cart cart)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", cart.ProductId);
        parameters.Add("UserId", cart.UserId);
        parameters.Add("Quantity", cart.Quantity);
        await connection.ExecuteAsync(_commandProvider.AddItemToCart(), parameters);
    }

    public override async Task<bool> IsItemInCartAsync(int productId, string userId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        parameters.Add("UserId", userId);
        return await connection.ExecuteScalarAsync<bool>(_commandProvider.IsItemInCart(), parameters);
    }

    public override async Task DeleteItemFromCartAsync(int productId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();
        parameters.Add("ProductId", productId);
        await connection.ExecuteAsync(_commandProvider.DeleteItemFromCart(), parameters);
    }

    public override async Task ClearCartAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(_commandProvider.ClearCart());
    }

    public override async Task<bool> UpdateCartItemQuantity(int productId, int change)
    {
        using var connection_db = await _connectionFactory.CreateConnectionAsync();

        var connection = (SqlConnection)connection_db;

        using var transaction =
            await connection.BeginTransactionAsync(IsolationLevel.Serializable, CancellationToken.None);

        try
        {
            var available = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT Quantity FROM Products WHERE Id = @productId",
                new { productId },
                transaction);

            var inCart = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT Quantity FROM Cart WHERE ProductId = @productId",
                new { productId },
                transaction);

            if (change > 0 && available <= 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            if (change < 0 && inCart <= 1)
            {
                await connection.ExecuteAsync(
                    "DELETE FROM Cart WHERE ProductId = @productId",
                    new { productId },
                    transaction);

                await connection.ExecuteAsync(
                    "UPDATE Products SET Quantity = Quantity + 1 WHERE Id = @productId",
                    new { productId },
                    transaction);
            }
            else
            {
                await connection.ExecuteAsync(
                    "UPDATE Cart SET Quantity = Quantity + @change WHERE ProductId = @productId",
                    new { productId, change },
                    transaction);

                await connection.ExecuteAsync(
                    "UPDATE Products SET Quantity = Quantity - @change WHERE Id = @productId",
                    new { productId, change },
                    transaction);
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await transaction.RollbackAsync();
            throw;
        }
    }
}