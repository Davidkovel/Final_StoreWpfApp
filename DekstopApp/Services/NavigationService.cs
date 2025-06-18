using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DekstopApp.ViewModels.Payment;
using Microsoft.Extensions.DependencyInjection;

namespace DekstopApp.Services;

public class NavigationService
{
    private record NavigationEntry(UserControl View, object ViewModel);

    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<NavigationEntry> _navigationStack = new();
    public event Action<UserControl>? OnNavigate;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TView, TViewModel>(Action<TViewModel> initViewModel = null)
        where TView : UserControl
        where TViewModel : class
    {
        var view = _serviceProvider.GetRequiredService<TView>();
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        view.DataContext = viewModel;

        initViewModel?.Invoke(viewModel);

        view.DataContext = viewModel;
        _navigationStack.Push(new NavigationEntry(view, viewModel));
        OnNavigate?.Invoke(view);
    }

    public void NavigateTo<TView>()
        where TView : UserControl
    {
        var view = _serviceProvider.GetRequiredService<TView>();
        _navigationStack.Push(new NavigationEntry(view, null)); // ViewModel = null
        OnNavigate?.Invoke(view);
    }
    
    public void NavigateToWindow<TWindow, TViewModel>()
        where TWindow : Window
        where TViewModel : class
    {
        var window = _serviceProvider.GetRequiredService<TWindow>();
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        window.DataContext = viewModel;
        window.Show();
    }

    public void NavigateBack()
    {
        if (_navigationStack.Count <= 1) return;

        // Удаляем текущую страницу
        _navigationStack.Pop();

        // Восстанавливаем предыдущую
        var previous = _navigationStack.Peek();
        previous.View.DataContext = previous.ViewModel;

        OnNavigate?.Invoke(previous.View);
    }
}