using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Entity;
using Core.Repository;
using Data.Abstractions.NoSqlDatabase;

namespace DekstopApp.Services;

public class ProductService(ProductRepository productRepository, ICacheProvider cacheProvider) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async void AddProduct(Product product)
    {
        await productRepository.AddProductAsync(product);
        OnPropertyChanged();
    }

    public async Task<IEnumerable<Product>> LoadProducts()
    {
        const string cacheKey = "products";

        var cachedProducts = await cacheProvider.GetAsync<IEnumerable<Product>>(cacheKey);
        if (cachedProducts != null)
            // Console.WriteLine("[DEBUG] Products from cache", cachedProducts);
            return cachedProducts;

        var products = await productRepository.GetProductsAsync();

        await cacheProvider.SetAsync(cacheKey, products, TimeSpan.FromMinutes(10));

        return products;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
    {
        string cacheKey =  $"products:category:{categoryId}";
        
        var cachedProducts = await cacheProvider.GetAsync<IEnumerable<Product>>(cacheKey);
        if (cachedProducts != null)
            return cachedProducts;

        var products = await productRepository.GetProductsByCategoryIdAsync(categoryId);

        await cacheProvider.SetAsync(cacheKey, products, TimeSpan.FromMinutes(10));
        
        return products;
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}