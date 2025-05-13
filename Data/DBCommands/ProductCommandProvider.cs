using Data.Abstractions.Database;

namespace Data.DBCommands;

public class ProductCommandProvider : IProductSqlCommandProvider
{
    private IProductSqlCommandProvider _productSqlCommandProviderImplementation;

    public string GetProducts() => @"
            SELECT 
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageUrl,
                p.CategoryId,
                p.Quantity,
                c.Name AS CategoryName
            FROM 
                Products p
            LEFT JOIN 
                Categories c ON p.CategoryId = c.Id
            ORDER BY 
                p.Name;
        ";

    public string GetProductsByCategory(int categoryId) => $@"
        SELECT 
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.CategoryId,
            p.Quantity
        FROM 
            Products p
        WHERE 
            p.CategoryId = {categoryId}
        ORDER BY 
            p.Name;";
}