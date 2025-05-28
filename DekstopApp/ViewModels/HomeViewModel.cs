using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using Data.Models;
using DekstopApp.Services;
using DekstopApp.Views;
using Microsoft.Extensions.Logging;

namespace DekstopApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ProductModel> _products = new();

    [ObservableProperty] private ObservableCollection<Category> _categories = new();

    private readonly ILogger<HomeViewModel> _logger;
    private readonly ProductService _productService;
    private readonly CategoryService _categoryService;
    private readonly NavigationService _navigationService;

    [ObservableProperty] private int? _selectedCategoryId;

    public HomeViewModel(
        ILogger<HomeViewModel> logger,
        ProductService productService,
        CategoryService categoryService,
        NavigationService navigationService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
        _navigationService = navigationService;

        LoadProductsCommand = new AsyncRelayCommand(LoadProducts);
        LoadCategoryCommand = new AsyncRelayCommand(LoadCategories);
    }

    public IAsyncRelayCommand LoadProductsCommand { get; }
    public IAsyncRelayCommand LoadCategoryCommand { get; }
    public IRelayCommand<ProductModel> ViewProductDetailCommand => new RelayCommand<ProductModel>(ViewProductDetail);

    private async Task LoadProducts()
    {
        try
        {
            _logger.LogInformation("Loading product...");
            IEnumerable<ProductModel> loadedProducts;

            if (SelectedCategoryId.HasValue)
            {
                loadedProducts = await _productService.GetProductsByCategory(SelectedCategoryId.Value);
            }
            else
            {
                loadedProducts = await _productService.LoadProducts();
            }

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
            _logger.LogInformation("Loading categories...");
            var loadedCategories = await _categoryService.GetCategories();

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

    private void ViewProductDetail(ProductModel? product)
    {
        if (product != null)
        {
            _navigationService.NavigateTo<DetailViewPage, DetailViewModel>(vm => vm.SelectedProduct = product);
        }
    }

    [RelayCommand]
    private async Task FilterByCategory(int? categoryId)
    {
        SelectedCategoryId = categoryId;
        await LoadProducts();
    }
}