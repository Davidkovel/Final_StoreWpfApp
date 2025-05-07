using Core.Entity;

namespace Core.Repository;

public abstract class CategoryRepository
{
    public abstract Task<IEnumerable<Category>> GetCategoriesAsync();
    public abstract Task AddCategoryAsync(Category category);
}