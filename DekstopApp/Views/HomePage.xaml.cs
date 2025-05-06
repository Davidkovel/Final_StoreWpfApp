using System.Windows.Controls;
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
        
        // Вызов команды загрузки продуктов
        if (viewModel.LoadProductsCommand.CanExecute(null))
            viewModel.LoadProductsCommand.Execute(null);
    }
}