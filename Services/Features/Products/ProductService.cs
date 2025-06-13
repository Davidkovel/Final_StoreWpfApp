using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Core.Entity;
using Core.Repository;
using Data.Abstractions.NoSqlDatabase;
using Data.Models;

namespace DekstopApp.Services;

public class ProductService(IMapper _mapper, ProductRepository productRepository, ICacheProvider cacheProvider)
    : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async void AddProduct(Product product)
    {
        await productRepository.AddProductAsync(product);
        OnPropertyChanged();
    }

    public async Task<IEnumerable<ProductModel>> LoadProducts()
    {
        const string cacheKey = "products";

        var cachedProducts = await cacheProvider.GetAsync<IEnumerable<ProductModel>>(cacheKey);
        if (cachedProducts != null)
            // Console.WriteLine("[DEBUG] Products from cache", cachedProducts);
            return cachedProducts;

        var products = await productRepository.GetProductsAsync();

        var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products);

        await cacheProvider.SetAsync(cacheKey, productsModel, TimeSpan.FromMinutes(10));

        return productsModel;
    }

    public async Task<IEnumerable<ProductModel>> GetProductsByCategory(int categoryId)
    {
        string cacheKey = $"products:category:{categoryId}";

        var cachedProducts = await cacheProvider.GetAsync<IEnumerable<ProductModel>>(cacheKey);
        if (cachedProducts != null)
            return cachedProducts;

        var products = await productRepository.GetProductsByCategoryIdAsync(categoryId);

        var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products);

        await cacheProvider.SetAsync(cacheKey, productsModel, TimeSpan.FromMinutes(10));

        return productsModel;
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}