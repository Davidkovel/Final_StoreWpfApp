using System.Configuration;
using System.Data;
using System.Windows;
using Core.Repository;
using Data.DBProvider;
using Data.Repository;
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

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, connectionString);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection serviceLocator, string connectionString)
    {
        serviceLocator.AddLogging();

        // Data Source
        serviceLocator.AddSingleton<IDatabaseProvider>(_ =>
            new SqlServerDatabaseProvider(connectionString));

        // Repositories
        serviceLocator.AddSingleton<ProductRepository, ProductRepositoryImpl>();
        serviceLocator.AddSingleton<CategoryRepository, CategoryRepositoryImpl>();

        // Register Services / Use Cases
        serviceLocator.AddSingleton<NavigationService>();
        serviceLocator.AddSingleton<ProductService>();
        serviceLocator.AddSingleton<CategoryService>();

        // Register ViewModels
        serviceLocator.AddSingleton<HomeViewModel>(sp => new HomeViewModel(
            logger: sp.GetRequiredService<ILogger<HomeViewModel>>(),
            productService: sp.GetRequiredService<ProductService>(),
            categoryService: sp.GetRequiredService<CategoryService>(),
            navigationService: sp.GetRequiredService<NavigationService>()
        ));

        // Register Views
        serviceLocator.AddSingleton<HomePage>(sp => new HomePage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<HomeViewModel>()
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