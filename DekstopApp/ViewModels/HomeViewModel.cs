using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;
using Microsoft.Extensions.Logging;

namespace DekstopApp.ViewModels;

public class HomeViewModel
{
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ICommand LoadProductsCommand { get; }
    public ICommand LoadCategoryCommand { get; }
    
    private readonly ILogger<HomeViewModel> _logger;
    private readonly ProductService _productService;
    private readonly CategoryService _categoryService;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly NavigationService _navigationService;


    public HomeViewModel(ILogger<HomeViewModel> logger, ProductService productService, CategoryService categoryService,
        NavigationService navigationService)
    {
        _navigationService = navigationService;
        _productService = productService;
        _categoryService = categoryService;
        _logger = logger;
        LoadProductsCommand = new AsyncRelayCommand(LoadProducts);
        LoadCategoryCommand = new AsyncRelayCommand(LoadCategories);
    }

    private async Task LoadProducts()
    {
        try
        {
            _logger.LogInformation("Loading product to main homepage.xaml...");
            var loadedProducts = await _productService.LoadProducts();

            Products.Clear();
            foreach (var product in loadedProducts)
            {
                Products.Add(product);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    private async Task LoadCategories()
    {
        try
        {
            _logger.LogInformation("Loading categories to homepage.xaml...");
            var loadedCategories = await _categoryService.GetCategories();
            Console.WriteLine($"Loaded {loadedCategories.Count()} categories"); 
            Categories.Clear(); 
            foreach (var category in loadedCategories)
            {
                Categories.Add(category);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}