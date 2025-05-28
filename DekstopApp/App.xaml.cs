using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Core.Repository;
using Data.Abstractions.Database;
using Data.Abstractions.NoSqlDatabase;
using Data.DBCommands;
using Data.DBProvider;
using Data.Infrastructure.Caching;
using Data.Repository;
using DekstopApp.Mapping;
using DekstopApp.Services;
using DekstopApp.ViewModels;
using DekstopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DekstopApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
///
// @Todo Исправить данный функционал с DI ригестрации
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var config = ConfigLoader.LoadConfig();

        var connectionString = ConfigLoader.GetConnectionDBString();
        var redisConnection = ConfigLoader.GetRedisConnectionString();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, connectionString, redisConnection);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection serviceLocator, string connectionString, string redisConnection)
    {
        serviceLocator.AddLogging();

        // Mapper
        serviceLocator.AddAutoMapper(typeof(MappingProfile));
        
        // Data Source
        serviceLocator.AddSingleton<IDatabaseProvider>(_ =>
            new SqlServerDatabaseProvider(connectionString));

        // Command Providers
        serviceLocator.AddSingleton<IProductSqlCommandProvider, ProductCommandProvider>();
        serviceLocator.AddSingleton<ICategorySqlCommandProvider, CategoryCommandProvider>();
        serviceLocator.AddSingleton<ICartSqlCommandProvider, CartCommandProvider>();

        // Infastructure
        serviceLocator.AddSingleton<ICacheProvider>(_ =>
            new RedisCacheProvider(redisConnection));

        // Repositories
        serviceLocator.AddSingleton<ProductRepository, ProductRepositoryImpl>();
        serviceLocator.AddSingleton<CategoryRepository, CategoryRepositoryImpl>();
        serviceLocator.AddSingleton<CartRepository, CartRepositoryImpl>();

        // Register Services / Use Cases
        serviceLocator.AddSingleton<NavigationService>();
        serviceLocator.AddSingleton<ProductService>();
        serviceLocator.AddSingleton<CategoryService>();
        serviceLocator.AddSingleton<CartService>();

        // Register ViewModels
        serviceLocator.AddSingleton<HomeViewModel>(sp => new HomeViewModel(
            logger: sp.GetRequiredService<ILogger<HomeViewModel>>(),
            productService: sp.GetRequiredService<ProductService>(),
            categoryService: sp.GetRequiredService<CategoryService>(),
            navigationService: sp.GetRequiredService<NavigationService>()
        ));

        serviceLocator.AddSingleton<DetailViewModel>(sp => new DetailViewModel(
            navigationService: sp.GetRequiredService<NavigationService>(),
            cartService: sp.GetRequiredService<CartService>()
        ));

        serviceLocator.AddSingleton<CartViewModel>(sp => new CartViewModel(
            logger: sp.GetRequiredService<ILogger<CartViewModel>>(),
            navigationService: sp.GetRequiredService<NavigationService>(),
            cartService: sp.GetRequiredService<CartService>()
        ));

        // Register Views
        serviceLocator.AddSingleton<HomePage>(sp => new HomePage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<HomeViewModel>()
        ));

        serviceLocator.AddSingleton<DetailViewPage>(sp => new DetailViewPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<DetailViewModel>()
        ));

        serviceLocator.AddSingleton<CartPage>(sp => new CartPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<CartViewModel>()
        ));

        // Register MainWindow
        serviceLocator.AddSingleton<MainWindow>(sp =>
        {
            var navigationService = sp.GetRequiredService<NavigationService>();
            var productService = sp.GetRequiredService<ProductService>();
            return new MainWindow(navigationService, productService);
        });
    }
}