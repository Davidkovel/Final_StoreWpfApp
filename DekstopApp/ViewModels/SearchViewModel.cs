using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;

namespace DekstopApp.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly ProductSearchService _productElasticSearchService;
    
    [ObservableProperty] private List<Product> _products = new();
    
    [ObservableProperty] private string _searchQuery = string.Empty;

    public SearchViewModel(ProductSearchService productSearchService)
    {
        _productElasticSearchService = productSearchService;
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
        {
            return;
        }

        Products = await _productElasticSearchService.Search(SearchQuery);
    }
}