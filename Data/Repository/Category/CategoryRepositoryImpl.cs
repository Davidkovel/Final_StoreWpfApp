using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entity;
using Core.Repository;
using Dapper;
using Data.Abstractions.Database;
using Data.Database.Abstractions;
using Data.DBCommands;
using Data.DBProvider;

namespace Data.Repository;

public class CategoryRepositoryImpl : CategoryRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICategorySqlCommandProvider _commandProvider;

    public CategoryRepositoryImpl(IConnectionFactory connectionFactory, ICategorySqlCommandProvider commandProvider)
    {
        _connectionFactory = connectionFactory;
        _commandProvider = commandProvider;
    }

    public override async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Category>(_commandProvider.GetCategories());
    }

    public override async Task AddCategoryAsync(Category category)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            @"INSERT INTO Categories 
              (Name, Description) 
              VALUES (@Name, @Description)",
            category);
    }
}