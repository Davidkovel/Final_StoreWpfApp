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
}