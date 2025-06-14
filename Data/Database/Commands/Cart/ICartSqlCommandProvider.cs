namespace Data.Abstractions.Database;

public interface ICartSqlCommandProvider
{
    public string GetCartItems();
    public string GetCartItemByUserId();
    public string AddItemToCart();
    public string IsItemInCart();
    public string DeleteItemFromCart();
    public string ClearCart();
    // public string IncrementItemQuantity();
    // public string DecrementItemQuantity();
}