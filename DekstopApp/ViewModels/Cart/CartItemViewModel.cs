using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;

namespace DekstopApp.ViewModels;

public partial class CartItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _quantity;
    public int ProductId { get; set; }
    public string ProductName { get; }
    public decimal Price { get; }
    public string ProductImageUrl { get; }

    private readonly CartService _cartService;
    
    public decimal ItemTotal => Quantity * Price;

    public CartItemViewModel(CartService cartService, Cart cartItem)
    {
        _cartService = cartService;
        
        ProductId = cartItem.ProductId;
        ProductName = cartItem.ProductName;
        Price = cartItem.ProductPrice;
        ProductImageUrl = cartItem.ProductImageUrl;
        _quantity = cartItem.Quantity;
        
        this.PropertyChanged += (_, e) => 
        {
            if (e.PropertyName == nameof(Quantity) || e.PropertyName == nameof(Price))
                OnPropertyChanged(nameof(ItemTotal));
        };
    }

    [RelayCommand]
    private async Task IncreaseQuantity()
    {
        //Console.WriteLine("Increase clicked");
        var success = await _cartService.UpdateCartItemQuantity(ProductId, +1);
        if (success)
            Quantity++;
    }

    [RelayCommand]
    private async Task DecreaseQuantity()
    {
        //Console.WriteLine("Decrease clicked");
        var success = await _cartService.UpdateCartItemQuantity(ProductId, -1);
        if (success && Quantity > 1)
            Quantity--;
    }
    
}
