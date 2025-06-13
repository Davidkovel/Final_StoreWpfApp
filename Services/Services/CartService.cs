using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Core.Entity;
using Core.Repository;
using Data.Models;

namespace DekstopApp.Services;

public class CartService(CartRepository cartRepository) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task<IEnumerable<Cart>> LoadCartItems()
    {
        var carts = await cartRepository.GetCartItemsAsync();
        return carts;
    }
    
    public async Task<IEnumerable<Cart>> LoadCartItemsByUserId(string userId)
    {
        var carts = await cartRepository.GetCartItemsByUserIdAsync(userId);
        return carts;
    }

    
    public async Task AddItemToCart(ProductModel product, string userId, int quantity=1)
    {
        var cart = new Cart
        {
            ProductId = product.Id,
            UserId = userId,
            Quantity = quantity,
            ProductName = product.Name,
            ProductPrice = product.Price,
            ProductImageUrl = product.ImageUrl
            
        };
        
        await cartRepository.AddItemToCartAsync(cart);
    }
    
    public async void DeleteItemFromCart(int productId)
    {
        await cartRepository.DeleteItemFromCartAsync(productId);
    }
    
    public async void ClearCart()
    {
        await cartRepository.ClearCartAsync();
    }
    
    public async Task<bool> UpdateCartItemQuantity(int productId, int change)
    {
        
        var sucess = await cartRepository.UpdateCartItemQuantity(productId, change);
        return sucess;
    }
}