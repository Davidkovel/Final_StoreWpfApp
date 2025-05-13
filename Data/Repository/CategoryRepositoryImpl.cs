using Core.Entity;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class CategoryRepositoryImpl : CategoryRepository
{
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ICategorySqlCommandProvider _commandProvider;

    public CategoryRepositoryImpl(IDatabaseProvider databaseProvider, ICategorySqlCommandProvider commandProvider)
    {
        _databaseProvider = databaseProvider;
        _commandProvider = commandProvider;
    }

    public override async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        using var connection = await _databaseProvider.CreateConnectionAsync();
        return await connection.QueryAsync<Category>(_commandProvider.GetCategories());
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