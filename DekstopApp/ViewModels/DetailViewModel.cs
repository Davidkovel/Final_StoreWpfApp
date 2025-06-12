using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Entity;
using Core.Repository;
using Data.Models;
using DekstopApp.Common;
using DekstopApp.Services;
using DekstopApp.Views;

namespace DekstopApp.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;
    private readonly CommentService _commentService;

    [ObservableProperty] private ProductModel? _selectedProduct;
    [ObservableProperty] private string _newCommentText = string.Empty;
    [ObservableProperty] private bool _canAddComment = true;
    [ObservableProperty] private IEnumerable<Comment> _comments = new List<Comment>();

    private readonly CartService _cartService;

    public DetailViewModel(NavigationService navigationService, CartService cartService, AuthService authService,
        CommentService commentService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;
        _commentService = commentService;
    }

    [RelayCommand]
    private async Task LoadComments()
    {
        if (SelectedProduct == null) return;

        Comments = await _commentService.GetCommentsByProductId(SelectedProduct.Id);
    }

    [RelayCommand]
    private async Task AddComment()
    {
        if (SelectedProduct == null || string.IsNullOrWhiteSpace(NewCommentText))
        {
            MessageBox.Show("Please enter comment text");
            return;
        }

        if (!_authService.IsLoggedIn)
        {
            MessageBox.Show("Please login to add comments");
            _navigationService.NavigateTo<LoginPage, AuthViewModel>();
            return;
        }

        var currentUser = _authService.GetCurrentUser();
        if (currentUser == null) return;

        bool hasLocalComment = Comments.Any(c => c.UserId == currentUser.Id);
        if (hasLocalComment)
        {
            MessageBox.Show("You already have a comment for this product");
            CanAddComment = false;
            return;
        }

        bool hasServerComment = await _commentService.CheckIfUserHasComment(currentUser.Id, SelectedProduct.Id);
        if (hasServerComment)
        {
            MessageBox.Show("You already commented this product before");
            CanAddComment = false;
            return;
        }

        var comment = new Comment
        {
            ProductId = SelectedProduct.Id,
            UserId = currentUser.Id,
            Text = NewCommentText,
        };

        bool success = await _commentService.AddComment(comment);
        if (success)
        {
            await LoadComments();
            MessageBox.Show("Comment added successfully!");
        }
    }

    [RelayCommand]
    private void AddToCart()
    {
        if (_selectedProduct == null) return;

        Console.WriteLine(!_authService.IsLoggedIn);
        if (!_authService.IsLoggedIn)
        {
            _navigationService.NavigateTo<LoginPage, AuthViewModel>();
            MessageBox.Show("You should log in then you can add product to cart");
            return;
        }

        var user = _authService.GetCurrentUser();
        if (user == null)
        {
            MessageBox.Show("User info not available. Please login in to your account or Sign Up.");
            return;
        }

        int userId = (int)user?.Id;

        _cartService.AddItemToCart(_selectedProduct, userId, 1);

        Console.WriteLine("Product added to cart: " + _selectedProduct.Name);
        WeakReferenceMessenger.Default.Send(new CartUpdatedMessage());
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateBack();
    }
}