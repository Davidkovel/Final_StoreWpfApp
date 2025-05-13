using Core.Entity;
using Core.Repository;
using Data.DBProvider;

namespace Data.Repository;

public class CartRepositoryImpl : CartRepository
{
    private readonly IDatabaseProvider _databaseProvider;
    private CartRepository _cartRepositoryImplementation;

    public CartRepositoryImpl(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
        _databaseProvider.InitializeDatabaseAsync();
    }

    public override Task<IEnumerable<Cart>> GetCartsAsync()
    {
        return _cartRepositoryImplementation.GetCartsAsync();
    }
}