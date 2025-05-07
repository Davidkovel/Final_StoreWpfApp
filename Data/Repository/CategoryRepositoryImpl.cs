using Core.Entity;
using Core.Repository;
using Dapper;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class CategoryRepositoryImpl : CategoryRepository
{
    private readonly IDatabaseProvider _databaseProvider;
    private CategoryRepository _categoryRepositoryImplementation;

    public CategoryRepositoryImpl(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
        _databaseProvider.InitializeDatabaseAsync();
    }
    public override async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Category>(DbCommands.GetCategories());
    }

    public override async Task AddCategoryAsync(Category category)
    { 
        using var connection = await _databaseProvider.CreateConnectionAsync();
        await connection.ExecuteAsync(
            @"INSERT INTO Categories 
              (Name, Description) 
              VALUES (@Name, @Description)",
            category);
    }
}