using System;
using System.Collections.ObjectModel;
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
using Services;
using Supabase.Gotrue;

namespace DekstopApp.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;
    private readonly CommentService _commentService;
    private readonly CartService _cartService;
    private readonly IDialogService _dialogService;

    [ObservableProperty] private ProductModel? _selectedProduct;
    [ObservableProperty] private string _newCommentText = string.Empty;
    [ObservableProperty] private bool _canAddComment = true;
    [ObservableProperty] private IEnumerable<Comment> _comments = new List<Comment>();

    public DetailViewModel(NavigationService navigationService, CartService cartService, AuthService authService,
        CommentService commentService, IDialogService dialogService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;
        _commentService = commentService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task LoadComments()
    {
        try
        {
            if (SelectedProduct == null) return;

            Comments = new ObservableCollection<Comment>(
                await _commentService.GetCommentsByProductId(SelectedProduct.Id));
        }
        catch (Exception ex)
        {
            string ErrorMessage = "Comments can't load. Try again. " + ex.Message;
            await _dialogService.ShowErrorMessage(ErrorMessage);
        }
    }

    [RelayCommand]
    private async Task AddComment()
    {
        if (!ValidateCommentInput()) return;
        if (!await ValidateUserAuthenticationAsync()) return;

        var currentUser = await GetCurrentUser();
        if (currentUser is null) return;

        if (await CheckExistingCommentsAsync(currentUser)) return;

        await SubmitNewCommentAsync(currentUser);
        // if (SelectedProduct == null || string.IsNullOrWhiteSpace(NewCommentText))
        // {
        //     MessageBox.Show("Please enter comment text");
        //     return;
        // }
        //
        // if (!_authService.IsLoggedIn)
        // {
        //     MessageBox.Show("Please login to add comments");
        //     _navigationService.NavigateTo<LoginPage, AuthViewModel>();
        //     return;
        // }
        //
        // var currentUser = _authService.GetCurrentUser();
        // if (currentUser == null) return;
        //
        // bool hasLocalComment = Comments.Any(c => c.UserId == currentUser.Result.Id);
        // if (hasLocalComment)
        // {
        //     MessageBox.Show("You already have a comment for this product");
        //     CanAddComment = false;
        //     return;
        // }
        //
        // bool hasServerComment = await _commentService.CheckIfUserHasComment(currentUser.Id, SelectedProduct.Id);
        // if (hasServerComment)
        // {
        //     MessageBox.Show("You already commented this product before");
        //     CanAddComment = false;
        //     return;
        // }
        //
        // var comment = new Comment
        // {
        //     ProductId = SelectedProduct.Id,
        //     UserId = currentUser.Result.Id,
        //     Text = NewCommentText,
        // };
        //
        // bool success = await _commentService.AddComment(comment);
        // if (success)
        // {
        //     await LoadComments();
        //     MessageBox.Show("Comment added successfully!");
        // }
    }

    [RelayCommand]
    private async Task AddToCart()
    {
        if (SelectedProduct is null) return;
        
        if (!await ValidateUserAuthenticationAsync()) return;
        
        var currentUser = await GetCurrentUser();
        if (currentUser is null) return;
        
        await ExecuteAddToCartAsync(currentUser);
        // if (_selectedProduct == null) return;
        //
        // Console.WriteLine(!_authService.IsLoggedIn);
        // if (!_authService.IsLoggedIn)
        // {
        //     _navigationService.NavigateTo<LoginPage, AuthViewModel>();
        //     MessageBox.Show("You should log in then you can add product to cart");
        //     return;
        // }
        //
        // var user = _authService.GetCurrentUser();
        // if (user == null)
        // {
        //     MessageBox.Show("User info not available. Please login in to your account or Sign Up.");
        //     return;
        // }
        //
        // string userId = user.Result.Id;
        //
        // _cartService.AddItemToCart(_selectedProduct, userId, 1);
        //
        // Console.WriteLine("Product added to cart: " + _selectedProduct.Name);
        // WeakReferenceMessenger.Default.Send(new CartUpdatedMessage());
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateBack();
    }

    // Private methods

    private bool ValidateCommentInput()
    {
        if (SelectedProduct is null || string.IsNullOrWhiteSpace(NewCommentText))
        {
            _dialogService.ShowMessage("Please enter comment text");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateUserAuthenticationAsync()
    {
        if (_authService.IsLoggedIn) return true;

        _dialogService.ShowMessage("Please login to continue");
        _navigationService.NavigateTo<LoginPage, AuthViewModel>();
        return false;
    }

    private async Task<User?> GetCurrentUser()
    {
        var user = await _authService.GetCurrentUser();
        if (user is not null) return user;

        _dialogService.ShowMessage("User info not available. Please login.");
        return null;
    }

    private async Task<bool> CheckExistingCommentsAsync(User user)
    {
        bool hasLocalComment = Comments.Any(c => c.UserId == user.Id);
        if (hasLocalComment)
        {
            _dialogService.ShowMessage("You already have a comment for this product");
            CanAddComment = false;
            return true;
        }

        bool hasServerComment = await _commentService.CheckIfUserHasComment(user.Id, SelectedProduct!.Id);
        if (hasServerComment)
        {
            _dialogService.ShowMessage("You already commented this product before");
            CanAddComment = false;
            return true;
        }

        return false;
    }

    private async Task SubmitNewCommentAsync(User user)
    {
        var comment = new Comment
        {
            ProductId = SelectedProduct!.Id,
            UserId = user.Id,
            Text = NewCommentText,
            CreatedAt = DateTime.UtcNow
        };

        bool success = await _commentService.AddComment(comment);
        if (success)
        {
            NewCommentText = string.Empty;
            await LoadComments();
            _dialogService.ShowMessage("Comment added successfully!");
        }
    }

    private async Task ExecuteAddToCartAsync(User user)
    {
        try
        {
            await _cartService.AddItemToCart(SelectedProduct!, user.Id, 1);
            WeakReferenceMessenger.Default.Send(new CartUpdatedMessage());
            _dialogService.ShowMessage($"{SelectedProduct!.Name} added to cart");
        }
        catch (Exception ex)
        {
            _dialogService.ShowErrorMessage("Failed to add product to cart" + ex.Message);
        }
    }
}