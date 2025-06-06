// using System.Collections.ObjectModel;
// using AutoMapper;
// using CommunityToolkit.Mvvm.ComponentModel;
// using CommunityToolkit.Mvvm.Input;
// using Core.Entity;
// using Data.Models;
// using DekstopApp.Services;
// using DekstopApp.Views;
//
// namespace DekstopApp.ViewModels;
//
// public partial class SearchViewModel : ObservableObject
// {
//     private readonly ProductSearchService _productElasticSearchService;
//     private readonly NavigationService _navigationService;
//     private readonly ProductService _productService;
//     private readonly IMapper _mapper;
//     private CancellationTokenSource _searchCts;
//
//
//     [ObservableProperty] private ObservableCollection<ProductModel> _products = new();
//
//     [ObservableProperty] private ObservableCollection<Product> _searchSuggestions = new();
//
//     [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
//     private string _searchQuery = "";
//
//
//     [ObservableProperty] private bool _isSearching;
//
//     public SearchViewModel(ProductSearchService productSearchService, NavigationService navigationService,
//         ProductService productService, IMapper mapper)
//     {
//         _productElasticSearchService = productSearchService;
//         _navigationService = navigationService;
//         _productService = productService;
//         _mapper = mapper;
//         
//         
//         Console.WriteLine($"Searching 2 for {_searchQuery}");
//         _ = _productElasticSearchService.InitializeIndex(); // asyncio.create_tsak
//         _ = LoadPopularProducts();
//     }
//
//     private async Task LoadPopularProducts()
//     {
//         Console.WriteLine("Loading popular products");
//         var productEntities = await _productService.LoadProducts();
//         // var productModels = _mapper.Map<List<ProductModel>>(productEntities);
//         Products = _mapper.Map<ObservableCollection<ProductModel>>(productEntities);
//     }
//
//     [RelayCommand]
//     private async Task Search()
//     {
//         Console.WriteLine($"Searching for {_searchQuery}");
//         if (string.IsNullOrWhiteSpace(SearchQuery))
//         {
//             await LoadPopularProducts();
//             return;
//         }
//
//         IsSearching = true;
//         try
//         {
//             var productEntities = await _productElasticSearchService.Search(SearchQuery);
//             Console.WriteLine(productEntities);
//             var productModels = _mapper.Map<List<ProductModel>>(productEntities);
//             Products = new ObservableCollection<ProductModel>(productModels);
//         }
//         finally
//         {
//             IsSearching = false;
//         }
//     }
//
//     [RelayCommand]
//     private void ShowProductDetails(Product product)
//     {
//         Console.WriteLine("Navigating to product details");
//         var ProductEntity = _mapper.Map<ProductModel>(product);
//         _navigationService.NavigateTo<DetailViewPage, DetailViewModel>(vm => vm.SelectedProduct = ProductEntity);
//     }
//
//     partial void OnSearchQueryChanged(string value)
//     {
//         Console.WriteLine($"Searching for {value}");
//         _searchCts?.Cancel();
//         _searchCts = new CancellationTokenSource();
//
//         if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
//         {
//             SearchSuggestions.Clear();
//             return;
//         }
//
//         _ = UpdateSuggestionsAsync(value, _searchCts.Token);
//     }
//
//     private async Task UpdateSuggestionsAsync(string query, CancellationToken ct)
//     {
//         try
//         {
//             Console.WriteLine($"Updating suggestions for {query}");
//             var suggestions = await _productElasticSearchService.GetSuggestions(query, 5, ct);
//             SearchSuggestions = new ObservableCollection<Product>(suggestions);
//         }
//         catch (OperationCanceledException)
//         {
//             // Поиск был отменен - ничего не делаем
//         }
//     }
// }