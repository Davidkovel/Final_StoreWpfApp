using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Entity;
using Core.Repository;

namespace DekstopApp.Services;

public class ProductService(ProductRepository productRepository) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public async void AddProduct(Product product)
    {
        await productRepository.AddProductAsync(product);
        OnPropertyChanged();
    }
    
    public async Task<IEnumerable<Product>> LoadProducts()
    {
        // var testProduct = new Product(
        //     name: "Test Product",
        //     description: "Test Description",
        //     price: 9.99m,
        //     imageUrl: "default.jpg",
        //     categoryId: 1,
        //     quantity: 10
        // );
        // await productRepository.AddProductAsync(testProduct);
        var products = await productRepository.GetProductsAsync();
        Console.WriteLine(products);
        return products;
    }
    
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}