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
    [ObservableProperty] private ObservableCollection<Cart> _cartItems = new();

    private readonly ILogger<CartViewModel> _logger;
    private readonly NavigationService _navigationService;
    private readonly CartService _cartService;

    public CartViewModel(ILogger<CartViewModel> logger, NavigationService navigationService, CartService cartService)
    {
        _logger = logger;
        _navigationService = navigationService;
        _cartService = cartService;

        WeakReferenceMessenger.Default.Register<CartUpdatedMessage>(this);

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

    [RelayCommand]
    private void DeleteCartItem(int ProductId)
    {
        try
        {
            Console.WriteLine("Deleting cart item...");
            _cartService.DeleteItemFromCart(ProductId);
            var cartItemToRemove = CartItems.FirstOrDefault(item => item.ProductId == ProductId);
            Console.WriteLine(cartItemToRemove);
            if (cartItemToRemove != null)
            {
                CartItems.Remove(cartItemToRemove);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    [RelayCommand]
    private void ClearCartItems()
    {
        try
        {
            _cartService.ClearCart();
            CartItems.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async void Receive(CartUpdatedMessage message)
    {
        // 🔁 Перезагружаем корзину, когда пришло сообщение
        await LoadCartItems();
    }
}