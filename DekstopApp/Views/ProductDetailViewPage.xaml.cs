using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;
using DekstopApp.ViewModels;

namespace DekstopApp.Views;

public partial class DetailViewPage : UserControl
{
    private readonly NavigationService _navigationService;

    //
    // [ObservableProperty] private Product _selectedProduct;
    //
    // [ObservableProperty] private int _quantity = 1;
    //
    public DetailViewPage(NavigationService navigationService, DetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
    }

    private void OnGoBackNavigationClick(object sender, System.Windows.RoutedEventArgs e)
    {
        _navigationService.NavigateBack();
    }

    //
    // [RelayCommand]
    // private void IncreaseQuantity()
    // {
    //     Quantity++;
    // }
    //
    // [RelayCommand]
    // private void DecreaseQuantity()
    // {
    //     if (Quantity > 1) Quantity--;
    // }

    // [RelayCommand]
    // private async Task AddToCart()
    // {
    //     await _cartService.AddToCartAsync(SelectedProduct, Quantity);
    //     _navigationService.GoBack();
    // }
}