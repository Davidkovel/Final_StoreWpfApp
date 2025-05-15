namespace Data.Abstractions.Database;

public interface ICartSqlCommandProvider
{
    public string GetCartItems();
    public string AddItemToCart();
    public string DeleteItemFromCart();
    public string ClearCart();
    // public string IncrementItemQuantity();
    // public string DecrementItemQuantity();
}