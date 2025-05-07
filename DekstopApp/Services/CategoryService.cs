using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Entity;
using Core.Repository;

namespace DekstopApp.Services;

public class CategoryService(CategoryRepository categoryRepository) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async void AddProduct(Category category)
    {
        await categoryRepository.AddCategoryAsync(category);
        OnPropertyChanged();
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        var categories = await categoryRepository.GetCategoriesAsync();
        return categories;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}