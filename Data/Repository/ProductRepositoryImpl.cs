using Core.Entity;
using Core.Repository;
using Dapper;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class ProductRepositoryImpl : ProductRepository
{
    private readonly IDatabaseProvider _databaseProvider;

    public ProductRepositoryImpl(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
        _databaseProvider.InitializeDatabaseAsync();
    }

    public override async Task<IEnumerable<Product>> GetProductsAsync()
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Product>(DbCommands.GetProducts());
    }

    public override async Task<Product> GetProductByIdAsync(int id)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id });
    }

    public override async Task AddProductAsync(Product product)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(
            @"INSERT INTO Products 
              (Name, Description, Price, ImageUrl, CategoryId, Quantity) 
              VALUES (@Name, @Description, @Price, @ImageUrl, @CategoryId, @Quantity)",
            product);
    }

    public override async Task UpdateProductAsync(Product product)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(
            @"UPDATE Products SET 
              Name = @Name, 
              Description = @Description, 
              Price = @Price, 
              ImageUrl = @ImageUrl, 
              CategoryId = @CategoryId, 
              Quantity = @Quantity,
              UpdatedAt = GETDATE()
              WHERE Id = @Id",
            product);
    }

    public override async Task DeleteProductAsync(int id)
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(
            "DELETE FROM Products WHERE Id = @Id",
            new { Id = id });
    }
}