using Core.Entity;

namespace Core.Repository;

public abstract class CartRepository
{
    public abstract Task<IEnumerable<Cart>> GetCartItemsAsync();
    public abstract Task AddItemToCartAsync(Cart cart);
    public abstract Task DeleteItemFromCartAsync(int productId);
    public abstract Task ClearCartAsync();
    public abstract Task<bool> UpdateCartItemQuantity(int productId, int change);
}