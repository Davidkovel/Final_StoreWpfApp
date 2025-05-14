using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;
using Microsoft.Extensions.Logging;

namespace DekstopApp.ViewModels;

public partial class CartViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Cart> _cartItems = new();

    private readonly ILogger<CartViewModel> _logger;
    private readonly NavigationService _navigationService;
    private readonly CartService _cartService;

    public CartViewModel(ILogger<CartViewModel> logger, NavigationService navigationService, CartService cartService)
    {
        _logger = logger;
        _navigationService = navigationService;
        _cartService = cartService;

        LoadCartItemsCommand = new AsyncRelayCommand(LoadCartItems);
    }

    public IAsyncRelayCommand LoadCartItemsCommand;

    private async Task LoadCartItems()
    {
        try
        {
            _logger.LogInformation("Loading cart items...");
            Console.WriteLine("Loading cart items...");
            IEnumerable<Cart> loadedCartItems;

            loadedCartItems = await _cartService.LoadCartItems();

            CartItems.Clear();

            foreach (var cartItem in loadedCartItems)
            {
                CartItems.Add(cartItem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex}");
        }
    }
}