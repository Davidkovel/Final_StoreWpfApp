using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using Core.Entity;
using DekstopApp.Services;
using DekstopApp.ViewModels;

namespace DekstopApp.Views;

public partial class HomePage : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;

    public HomePage(NavigationService navigationService, HomeViewModel viewModel, AuthService authService)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;
        _authService = authService;

        _authService.AuthStateChanged += UpdateAuthButtons;
        UpdateAuthButtons();

        if (viewModel.LoadProductsCommand.CanExecute(null))
            viewModel.LoadProductsCommand.Execute(null);

        if (viewModel.LoadCategoryCommand.CanExecute(null))
            viewModel.LoadCategoryCommand.Execute(null);
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
}


// batison282@cigidea.com 123KK!!!