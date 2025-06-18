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

    public string GetCartItemByUserId() => @"
        SELECT
            c.Id,
            c.ProductId,
            c.Quantity,
            p.Name AS ProductName,
            p.Price AS ProductPrice,
            p.ImageUrl as ProductImageUrl
        FROM Cart c
        INNER JOIN Products p ON c.ProductId = p.Id
        WHERE UserId = @UserId
        ORDER BY
            c.ProductId;
        ";
    

    public string AddItemToCart() => @"
        INSERT INTO Cart (ProductId, UserId, Quantity)
        VALUES (@ProductId, @UserId, @Quantity);";

    public string IsItemInCart()
    {
        return @"
            SELECT COUNT(*)
            FROM Cart
            WHERE ProductId = @ProductId AND UserId = @UserId;
            ";
    }

    public string DeleteItemFromCart() => @"
        DELETE FROM Cart
        WHERE ProductId = @ProductId;";
    
    public string ClearCart() => @"
        DELETE FROM Cart;";
    
    // public string IncrementItemQuantity() => @"
    //     UPDATE Cart
    //     SET Quantity = Quantity + 1
    //     WHERE ProductId = @ProductId;";
    //
    // public string DecrementItemQuantity() => @"
    //     UPDATE Cart
    //     SET Quantity = Quantity - 1
    //     WHERE ProductId = @ProductId;";
    //
}