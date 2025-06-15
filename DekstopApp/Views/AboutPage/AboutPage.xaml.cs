using System.Windows;
using System.Windows.Controls;
using DekstopApp.Services;
using DekstopApp.ViewModels;

namespace DekstopApp.Views;

public partial class AboutPage : UserControl
{
    private readonly NavigationService _navigationService;
    
    public AboutPage(NavigationService navigationService)
    {
        InitializeComponent();

        _navigationService = navigationService;
    }
    
    private void OnGoCartPageNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<CartPage, CartViewModel>();
    }

    private void OnGoLoginPageNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<LoginPage, AuthViewModel>();
    }
    
    private void OnGoHomePageNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<HomePage, HomeViewModel>();
    }
}