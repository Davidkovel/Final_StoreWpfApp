using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Data.Models;
using DekstopApp.Common;
using DekstopApp.Services;

namespace DekstopApp.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;

    [ObservableProperty] private ProductModel? _selectedProduct;

    private readonly CartService _cartService;

    public DetailViewModel(NavigationService navigationService, CartService cartService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
    }

    [RelayCommand]
    private void AddToCart()
    {
        if (_selectedProduct == null) return;

        _cartService.AddItemToCart(_selectedProduct, 1);

        Console.WriteLine("Product added to cart: " + _selectedProduct.Name);
        WeakReferenceMessenger.Default.Send(new CartUpdatedMessage());
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateBack();
    }
}