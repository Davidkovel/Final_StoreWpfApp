using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Core.Repository;
using Data.Abstractions.Database;
using Data.Abstractions.NoSqlDatabase;
using Data.Database.Abstractions;
using Data.Database.Providers;
using Data.DBCommands;
using Data.DBProvider;
using Data.DBProvider.SupabaseRemote;
using Data.Infrastructure.Caching;
using Data.Repository;
using DekstopApp.Mapping;
using DekstopApp.Services;
using DekstopApp.ViewModels;
using DekstopApp.ViewModels.Payment;
using DekstopApp.Views;
using DekstopApp.Views.Payment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;
using Services.Features.Payment;

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
        var supabaseConnectionStrings = ConfigLoader.GetSupabaseConnectionStrings();

        string supabaseApiKey = supabaseConnectionStrings[0];
        string supabaseEndpoint = supabaseConnectionStrings[1];

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, connectionString, redisConnection, supabaseApiKey, supabaseEndpoint);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection serviceLocator, string connectionString, string redisConnection,
        string supabaseApiKey, string supabaseEndpoint)
    {
        serviceLocator.AddLogging();

        // Mapper
        serviceLocator.AddAutoMapper(typeof(MappingProfile));

        // Data Source
        serviceLocator.AddSingleton<IDatabaseProvider>(_ =>
            new SqlServerDatabaseProvider(connectionString));

        serviceLocator.AddSingleton<IConnectionFactory>(provider =>
        {
            var dbProvider = provider.GetRequiredService<IDatabaseProvider>();
            return new SqlConnectionFactory(
                connectionString, dbProvider.InitializationTask);
        });

        serviceLocator.AddSingleton<ISupabaseRemoteProvider>(_ =>
            new SupabaseRemoteProvider(supabaseApiKey, supabaseEndpoint));

        // Command Providers
        serviceLocator.AddSingleton<IProductSqlCommandProvider, ProductCommandProvider>();
        serviceLocator.AddSingleton<ICategorySqlCommandProvider, CategoryCommandProvider>();
        serviceLocator.AddSingleton<ICartSqlCommandProvider, CartCommandProvider>();
        serviceLocator.AddSingleton<ICommentSqlCommandProvider, CommentCommandProvider>();

        // Infastructure
        serviceLocator.AddSingleton<ICacheProvider>(_ =>
            new RedisCacheProvider(redisConnection));

        // Repositories
        serviceLocator.AddSingleton<ProductRepository, ProductRepositoryImpl>();
        serviceLocator.AddSingleton<CategoryRepository, CategoryRepositoryImpl>();
        serviceLocator.AddSingleton<CartRepository, CartRepositoryImpl>();
        serviceLocator.AddSingleton<IAuthRepository, AuthRepository>();
        serviceLocator.AddSingleton<CommentRepository, CommentRepositoryImpl>();

        // Register Services / Use Cases
        serviceLocator.AddSingleton<NavigationService>();
        serviceLocator.AddSingleton<ProductService>();
        serviceLocator.AddSingleton<CategoryService>();
        serviceLocator.AddSingleton<CartService>();
        serviceLocator.AddSingleton<AuthService>();
        serviceLocator.AddSingleton<CommentService>();
        serviceLocator.AddSingleton<MonobankService>();
        serviceLocator.AddSingleton<IDialogService, DialogService>();

        // Register ViewModels
        serviceLocator.AddSingleton<HomeViewModel>(sp => new HomeViewModel(
            logger: sp.GetRequiredService<ILogger<HomeViewModel>>(),
            productService: sp.GetRequiredService<ProductService>(),
            categoryService: sp.GetRequiredService<CategoryService>(),
            navigationService: sp.GetRequiredService<NavigationService>()
        ));

        serviceLocator.AddSingleton<DetailViewModel>(sp => new DetailViewModel(
            navigationService: sp.GetRequiredService<NavigationService>(),
            cartService: sp.GetRequiredService<CartService>(),
            authService: sp.GetRequiredService<AuthService>(),
            commentService: sp.GetRequiredService<CommentService>(),
            dialogService: sp.GetRequiredService<IDialogService>()
        ));

        serviceLocator.AddSingleton<CartViewModel>(sp => new CartViewModel(
            logger: sp.GetRequiredService<ILogger<CartViewModel>>(),
            navigationService: sp.GetRequiredService<NavigationService>(),
            cartService: sp.GetRequiredService<CartService>(),
            authService: sp.GetRequiredService<AuthService>()
        ));

        serviceLocator.AddSingleton<AuthViewModel>(sp => new AuthViewModel(
            logger: sp.GetRequiredService<ILogger<AuthViewModel>>(),
            authService: sp.GetRequiredService<AuthService>()
        ));

        serviceLocator.AddSingleton<PaymentViewModel>(sp => new PaymentViewModel(
            cartService: sp.GetRequiredService<CartService>(),
            authService: sp.GetRequiredService<AuthService>(),
            monobankService: sp.GetRequiredService<MonobankService>(),
            dialogService: sp.GetRequiredService<DialogService>()
        ));
        
        // Register Views
        serviceLocator.AddSingleton<HomePage>(sp => new HomePage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<HomeViewModel>(),
            authService: sp.GetRequiredService<AuthService>()
        ));

        serviceLocator.AddSingleton<DetailViewPage>(sp => new DetailViewPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<DetailViewModel>(),
            authService: sp.GetRequiredService<AuthService>()
        ));

        serviceLocator.AddSingleton<CartPage>(sp => new CartPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            viewModel: sp.GetRequiredService<CartViewModel>()
        ));

        serviceLocator.AddSingleton<LoginPage>(sp => new LoginPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            authViewModel: sp.GetRequiredService<AuthViewModel>()
        ));

        serviceLocator.AddSingleton<RegisterPage>(sp => new RegisterPage(
            navigationService: sp.GetRequiredService<NavigationService>(),
            authViewModel: sp.GetRequiredService<AuthViewModel>()
        ));

        serviceLocator.AddSingleton<PaymentWindow>(sp => new PaymentWindow(
            viewModel: sp.GetRequiredService<PaymentViewModel>()
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