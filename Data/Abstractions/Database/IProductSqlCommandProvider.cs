namespace Data.Abstractions.Database;

public interface IProductSqlCommandProvider
{
    public string GetProducts();
    public string GetProductsByCategory(int categoryId);
}