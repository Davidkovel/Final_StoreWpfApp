using Data.Abstractions.Database;

namespace Data.DBCommands;

public class CartCommandProvider : ICartSqlCommandProvider
{
    private ICartSqlCommandProvider _cartSqlCommandProviderImplementation;

    public string GetCartItems() => @"
        SELECT
            c.Id,
            c.ProductId,
            c.Quantity,
            p.Name AS ProductName,
            p.Price AS ProductPrice,
            p.ImageUrl as ProductImageUrl
        FROM Cart c
        INNER JOIN Products p ON c.ProductId = p.Id
        ORDER BY 
            c.ProductId;";
}