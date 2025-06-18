using System.Windows;
using System.Windows.Controls;
using DekstopApp.Services;
using DekstopApp.ViewModels;
using DekstopApp.ViewModels.Payment;
using DekstopApp.Views.Payment;

namespace DekstopApp.Views;

public partial class CartPage : UserControl
{
    private readonly NavigationService _navigationService;

    public CartPage(NavigationService navigationService, CartViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;

        if (viewModel.LoadCartItemsCommand.CanExecute(null))
        {
            viewModel.LoadCartItemsCommand.Execute(null);
        }
    }

    private void OnGoHomePageNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<HomePage, HomeViewModel>();
    }

    private void OnGoAboutUsNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateTo<AboutPage>();
    }

    private void OnGoPaymenyNavigationClick(object sender, RoutedEventArgs e)
    {
        _navigationService.NavigateToWindow<PaymentWindow, PaymentViewModel>();
    }
}
