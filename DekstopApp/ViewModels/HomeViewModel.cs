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
    public ICommand LoadProductsCommand { get; }
    private readonly ILogger<HomeViewModel> _logger;
    private readonly ProductService _productService;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly NavigationService _navigationService;
    


    public HomeViewModel(ILogger<HomeViewModel> logger, ProductService productService,
        NavigationService navigationService)
    {
        _navigationService = navigationService;
        _productService = productService;
        _logger = logger;
        LoadProductsCommand = new AsyncRelayCommand(LoadProducts);
    }

    private async Task LoadProducts()
    {
        try
        {
            _logger.LogInformation("Loading product to main window.xaml...");
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

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}