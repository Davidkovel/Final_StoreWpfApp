using Core.Entity;

namespace Core.Repository;

public abstract class CartRepository
{
    public abstract Task<IEnumerable<Cart>> GetCartItemsAsync();
    
}