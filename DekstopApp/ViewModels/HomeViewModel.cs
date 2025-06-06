using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
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
    private readonly ProductSearchService _productElasticSearchService;
    private readonly NavigationService _navigationService;
    private readonly IMapper _mapper;
    private CancellationTokenSource _searchCts;


    [ObservableProperty] private int? _selectedCategoryId;

    [ObservableProperty] private ObservableCollection<Product> _searchSuggestions = new();

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string _searchQuery = "";

    [ObservableProperty] private bool _isSearching;

    public HomeViewModel(
        ILogger<HomeViewModel> logger,
        ProductService productService,
        CategoryService categoryService,
        ProductSearchService productElasticSearchService,
        NavigationService navigationService,
        IMapper mapper)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
        _productElasticSearchService = productElasticSearchService;
        _navigationService = navigationService;
        _mapper = mapper;

        LoadProductsCommand = new AsyncRelayCommand(LoadProducts);
        LoadCategoryCommand = new AsyncRelayCommand(LoadCategories);

        _ = _productElasticSearchService.InitializeIndex(); // asyncio.create_tsak
        _ = LoadPopularProducts();
    }

    public IAsyncRelayCommand LoadProductsCommand { get; }
    public IAsyncRelayCommand LoadCategoryCommand { get; }
    public IRelayCommand<ProductModel> ViewProductDetailCommand => new RelayCommand<ProductModel>(ViewProductDetail);


    private async Task LoadPopularProducts()
    {
        var productEntities = await _productService.LoadProducts();
        // var productModels = _mapper.Map<List<ProductModel>>(productEntities);
        Products = _mapper.Map<ObservableCollection<ProductModel>>(productEntities);
    }

    partial void OnSearchQueryChanged(string value)
    {
        Console.WriteLine($"Searching for {value}");
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
        {
            SearchSuggestions.Clear();
            return;
        }

        _ = UpdateSuggestionsAsync(value, _searchCts.Token);
    }

    private async Task UpdateSuggestionsAsync(string query, CancellationToken ct)
    {
        try
        {
            Console.WriteLine($"Updating suggestions for {query}");
            var suggestions = await _productElasticSearchService.GetSuggestions(query, 5, ct);
            SearchSuggestions = new ObservableCollection<Product>(suggestions);
        }
        catch (OperationCanceledException)
        {
            // Поиск был отменен - ничего не делаем
        }
    }

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
    private async Task Search()
    {
        Console.WriteLine($"Searching for {_searchQuery}");
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadPopularProducts();
            return;
        }

        IsSearching = true;
        try
        {
            var productEntities = await _productElasticSearchService.Search(SearchQuery);

            // Вывод всех найденных продуктов
            if (productEntities.Any())
            {
                Console.WriteLine("Found Products:");
                foreach (var p in productEntities)
                {
                    Console.WriteLine($"- {p.Name} | {p.Description} | {p.Price}₽");
                }
            }
            else
            {
                Console.WriteLine("No products found.");
            }

            var productModels = _mapper.Map<List<ProductModel>>(productEntities);
            Products = new ObservableCollection<ProductModel>(productModels);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void ShowProductDetails(Product product)
    {
        Console.WriteLine("Navigating to product details");
        var ProductEntity = _mapper.Map<ProductModel>(product);
        _navigationService.NavigateTo<DetailViewPage, DetailViewModel>(vm => vm.SelectedProduct = ProductEntity);
    }

    [RelayCommand]
    private async Task FilterByCategory(int? categoryId)
    {
        SelectedCategoryId = categoryId;
        await LoadProducts();
    }
}