using Data.Abstractions.Database;

namespace Data.DBCommands;

public class CategoryCommandProvider : ICategorySqlCommandProvider
{
    public string GetCategories() => @"
         SELECT
             c.Id,
             c.Name,
             c.Description
         FROM Categories c
         ORDER BY 
             c.Name;
         ";
}