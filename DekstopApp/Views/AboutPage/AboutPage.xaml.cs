using System.Windows;
using System.Windows.Controls;
using DekstopApp.Services;
using DekstopApp.ViewModels;

namespace DekstopApp.Views;

public partial class AboutPage : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;
    
    public AboutPage(NavigationService navigationService, AuthService authService)
    {
        InitializeComponent();

        _navigationService = navigationService;
        _authService = authService;
        
        _authService.AuthStateChanged += UpdateAuthButtons;
        UpdateAuthButtons();
    }
    
    private void UpdateAuthButtons()
    {
        if (_authService.IsLoggedIn)
        {
            LoginButton.Visibility = Visibility.Collapsed;
            ProfileButton.Visibility = Visibility.Visible;
        }
        else
        {
            LoginButton.Visibility = Visibility.Visible;
            ProfileButton.Visibility = Visibility.Collapsed;
        }
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