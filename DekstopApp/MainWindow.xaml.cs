using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using DekstopApp.Services;
using DekstopApp.ViewModels;
using DekstopApp.Views;

namespace DekstopApp;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private readonly ProductService _productService;

    public MainWindow(NavigationService navigationService, ProductService productService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        _productService = productService;
        
        _navigationService.OnNavigate += SetContent;
        _productService.PropertyChanged += OnAuthServicePropertyChanged!;
    }

    private void OnAuthServicePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        _navigationService.NavigateTo<HomePage, HomeViewModel>();
    }

    private void SetContent(UserControl content)
    {
        this.Content = content;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<HomePage, HomeViewModel>();
    }
}