using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using DekstopApp.Common;
using DekstopApp.Services;
using Microsoft.Extensions.Logging;

namespace DekstopApp.ViewModels;

public partial class CartViewModel : ObservableObject, IRecipient<CartUpdatedMessage>
{
    [ObservableProperty] private ObservableCollection<CartItemViewModel> _cartItems = new();

    private readonly ILogger<CartViewModel> _logger;
    private readonly NavigationService _navigationService;
    private readonly CartService _cartService;
    private readonly AuthService _authService;

    [ObservableProperty] private int _quantity;
    [ObservableProperty] private int totalPrice;
    [ObservableProperty] private decimal _total;


    public CartViewModel(ILogger<CartViewModel> logger, NavigationService navigationService, CartService cartService,
        AuthService authService)
    {
        _logger = logger;
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;

        WeakReferenceMessenger.Default.Register<CartUpdatedMessage>(this);

        LoadCartItemsCommand = new AsyncRelayCommand(LoadCartItems);
    }

    public IAsyncRelayCommand LoadCartItemsCommand;

    [RelayCommand]
    private async Task DeleteCartItem(int ProductId)
    {
        try
        {
            Console.WriteLine("Deleting cart item...");
            await DeleteCartItemAsync(ProductId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    [RelayCommand]
    private async Task ClearCartItems()
    {
        try
        {
            await ClearCartItemsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private async Task ClearCartItemsAsync()
    {
        _cartService.ClearCart();
        CartItems.Clear();
    }

    private async Task DeleteCartItemAsync(int ProductId)
    {
        _cartService.DeleteItemFromCart(ProductId);
        var cartItemToRemove = CartItems.FirstOrDefault(item => item.ProductId == ProductId);
        Console.WriteLine(cartItemToRemove);
        if (cartItemToRemove != null)
        {
            CartItems.Remove(cartItemToRemove);
        }
    }


    private async Task LoadCartItems()
    {
        try
        {
            _logger.LogInformation("Loading cart items...");
            Console.WriteLine("Loading cart items...");
            IEnumerable<Cart> loadedCartItems;

            var currentUser = _authService.GetCurrentUser();

            loadedCartItems = await _cartService.LoadCartItemsByUserId(currentUser.Result.Id);

            CartItems.Clear();

            foreach (var cartItem in loadedCartItems)
            {
                var cartItemViewModel = new CartItemViewModel(_cartService, cartItem);
                CartItems.Add(cartItemViewModel);
            }

            foreach (var item in CartItems)
            {
                item.PropertyChanged += (_, _) => UpdateTotal();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex}");
        }
    }

    private void UpdateTotal()
    {
        Total = CartItems.Sum(item => item.ItemTotal);
    }

    public async void Receive(CartUpdatedMessage message)
    {
        // 🔁 Перезагружаем корзину, когда пришло сообщение
        await LoadCartItems();
    }
}