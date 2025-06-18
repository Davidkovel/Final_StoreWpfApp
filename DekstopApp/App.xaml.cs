using System.Diagnostics;
using System.Windows;
using Core.Repository;
using Data.Abstractions.Database;
using Data.Abstractions.NoSqlDatabase;
using Data.Database.Abstractions;
using Data.Database.Commands.Rating;
using Data.Database.Providers;
using Data.DBCommands;
using Data.DBProvider;
using Data.DBProvider.SupabaseRemote;
using Data.Infrastructure.Caching;
using Data.Infrastructure.Queue;
using Data.Repository;
using Data.Repository.Rating;
using DekstopApp.Mapping;
using DekstopApp.Services;
using DekstopApp.Utils;
using DekstopApp.ViewModels;
using DekstopApp.ViewModels.Payment;
using DekstopApp.Views;
using DekstopApp.Views.Payment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;
using Services;
using Services.Features.Payment;
using Services.Features.Rating;
using Services.Features.Rating.Workers;

namespace DekstopApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
///
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    private MetricServer _metricServer;

    protected override void OnStartup(StartupEventArgs e)
    {
        var config = ConfigLoader.LoadConfig();

        var connectionString = ConfigLoader.GetConnectionDBString();
        var redisConnection = ConfigLoader.GetRedisConnectionString();
        var supabaseConnectionStrings = ConfigLoader.GetSupabaseConnectionStrings();

        _metricServer = new MetricServer(
            port: 5000
        );
        _metricServer.Start();

        // C:\Windows\System32>netsh http add urlacl url=http://+:5000/metrics/ user=Все
        //
        // Резервирование URL-адрес добавлено успешно
        //
        //
        // C:\Windows\System32>


        string supabaseApiKey = supabaseConnectionStrings[0];
        string supabaseEndpoint = supabaseConnectionStrings[1];

        StartMemoryUsageCollection();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, connectionString, redisConnection, supabaseApiKey, supabaseEndpoint);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var ratingWorker = _serviceProvider.GetRequiredService<RatingWorker>();
        Task.Run(() => ratingWorker.ExecuteAsync(CancellationToken.None));

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
        serviceLocator.AddSingleton<IRatingSqlCommandProvider, RatingCommandProvider>();

        // Infastructure
        serviceLocator.AddSingleton<ICacheProvider>(_ =>
            new RedisCacheProvider(redisConnection));

        // serviceLocator.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        // Repositories
        serviceLocator.AddSingleton<ProductRepository, ProductRepositoryImpl>();
        serviceLocator.AddSingleton<CategoryRepository, CategoryRepositoryImpl>();
        serviceLocator.AddSingleton<CartRepository, CartRepositoryImpl>();
        serviceLocator.AddSingleton<IAuthRepository, AuthRepository>();
        serviceLocator.AddSingleton<CommentRepository, CommentRepositoryImpl>();
        serviceLocator.AddSingleton<IRatingRepository, RatingRepositoryImpl>();

        // Register Services / Use Cases
        serviceLocator.AddSingleton<NavigationService>();
        serviceLocator.AddSingleton<ProductService>();
        serviceLocator.AddSingleton<CategoryService>();
        serviceLocator.AddSingleton<CartService>();
        serviceLocator.AddSingleton<AuthService>();
        serviceLocator.AddSingleton<CommentService>();
        serviceLocator.AddSingleton<RatingService>();
        serviceLocator.AddSingleton<MonobankService>();
        serviceLocator.AddSingleton<IDialogService, DialogService>();

        // Register Background Workers
        serviceLocator.AddSingleton<RatingWorker>();

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
            ratingService: sp.GetRequiredService<RatingService>(),
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
            dialogService: sp.GetRequiredService<IDialogService>()
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

        serviceLocator.AddSingleton<AboutPage>(sp => new AboutPage(
            navigationService: sp.GetRequiredKeyedService<NavigationService>(null),
            authService: sp.GetRequiredService<AuthService>()
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

    private void StartMemoryUsageCollection()
    {
        var timer = new Timer(_ =>
        {
            var process = Process.GetCurrentProcess();
            AppMetrics.MemoryUsage.Set(process.WorkingSet64 / 1024 / 1024); // MB
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _metricServer?.Stop();
        _metricServer?.Dispose();
        base.OnExit(e);
    }
}