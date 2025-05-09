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

    public HomePage(NavigationService navigationService, HomeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _navigationService = navigationService;

        if (viewModel.LoadProductsCommand.CanExecute(null))
            viewModel.LoadProductsCommand.Execute(null);

        if (viewModel.LoadCategoryCommand.CanExecute(null))
            viewModel.LoadCategoryCommand.Execute(null);
    }

    // private void ViewProductDetailNavigationClick(Product product)
    // {
    //     _navigationService.NavigateTo<DetailViewPage, DetailViewModel>(vm => vm.SelectedProduct = product);
    // }
}
