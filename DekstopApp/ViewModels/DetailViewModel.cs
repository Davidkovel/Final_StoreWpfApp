using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Data.Models;
using DekstopApp.Common;
using DekstopApp.Services;
using DekstopApp.Views;

namespace DekstopApp.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;

    [ObservableProperty] private ProductModel? _selectedProduct;

    private readonly CartService _cartService;

    public DetailViewModel(NavigationService navigationService, CartService cartService, AuthService authService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;
    }

    [RelayCommand]
    private void AddToCart()
    {
        if (_selectedProduct == null) return;
        
        Console.WriteLine(!_authService.IsLoggedIn);
        if (!_authService.IsLoggedIn)
        {
            _navigationService.NavigateTo<LoginPage, AuthViewModel>();
            MessageBox.Show("You should log in then you can add product to cart");
            return;
        }

        var user = _authService.GetCurrentUser();
        if (user == null)
        {
            MessageBox.Show("User info not available. Please login in to your account or Sign Up.");
            return;
        }
        int userId = (int)user?.Id;

        _cartService.AddItemToCart(_selectedProduct, userId, 1);

        Console.WriteLine("Product added to cart: " + _selectedProduct.Name);
        WeakReferenceMessenger.Default.Send(new CartUpdatedMessage());
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateBack();
    }
}