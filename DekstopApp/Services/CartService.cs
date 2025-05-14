using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Core.Entity;
using Core.Repository;

namespace DekstopApp.Services;

public class CartService(CartRepository cartRepository) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task<IEnumerable<Cart>> LoadCartItems()
    {
        var carts = await cartRepository.GetCartItemsAsync();
        return carts;
    }
    
    public async void AddItemToCart(Product product, int quantity=1)
    {
        var cart = new Cart
        {
            ProductId = product.Id,
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
}