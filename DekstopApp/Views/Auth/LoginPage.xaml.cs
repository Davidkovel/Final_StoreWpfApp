using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using DekstopApp.Services;
using DekstopApp.Utils;
using DekstopApp.ViewModels;
using Prometheus;

namespace DekstopApp.Views;

public partial class LoginPage : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly AuthViewModel _authViewModel;

    public LoginPage(NavigationService navigationService, AuthViewModel authViewModel)
    {
        InitializeComponent();
        DataContext = authViewModel;

        _navigationService = navigationService;
        _authViewModel = authViewModel;
    }

    private async void OnLoginButtonClick(object sender, RoutedEventArgs e)
    {
        using (AppMetrics.LoginDuration.NewTimer())
        {

            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(_authViewModel.Email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email and password cannot be empty",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                await _authViewModel.Login(_authViewModel.Email, password);

                _navigationService.NavigateTo<HomePage, HomeViewModel>();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private void OnRegisterNavigationClick(object sender, RoutedEventArgs e)
    {
        _authViewModel.Email = string.Empty;
        _authViewModel.Password = string.Empty;
        _navigationService.NavigateTo<RegisterPage, AuthViewModel>();
    }

    private void OnGoBackNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateBack();
    }
}