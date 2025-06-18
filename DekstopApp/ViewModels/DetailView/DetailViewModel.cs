using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
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
using Services.Features.Rating;
using Supabase.Gotrue;

namespace DekstopApp.ViewModels;

public partial class DetailViewModel : ObservableObject
{
    private readonly NavigationService _navigationService;
    private readonly AuthService _authService;
    private readonly CommentService _commentService;
    private readonly CartService _cartService;
    private readonly RatingService _ratingService;
    private readonly IDialogService _dialogService;

    [ObservableProperty] private ProductModel? _selectedProduct;
    [ObservableProperty] private string _newCommentText = string.Empty;
    [ObservableProperty] private bool _canAddComment = true;
    [ObservableProperty] private IEnumerable<Comment> _comments = new List<Comment>();
    private int _selectedRating = 1;
    [ObservableProperty] private double _averageRating;
    private List<int>_ratings = new List<int>();

    public DetailViewModel(NavigationService navigationService, CartService cartService, AuthService authService,
        CommentService commentService, RatingService ratingService, IDialogService dialogService)
    {
        _navigationService = navigationService;
        _cartService = cartService;
        _authService = authService;
        _commentService = commentService;
        _ratingService = ratingService;
        _dialogService = dialogService;
    }

    public int SelectedRating
    {
        get => _selectedRating;
        set
        {
            if (SetProperty(ref _selectedRating, value))
            {
                Console.WriteLine($"Selected rating changed to: {value}");
                // Дополнительная логика при изменении рейтинга
            }
        }
    }

    // Commands

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
    private async Task LoadRatings()
    {
        try
        {
            if (SelectedProduct == null) return;
            await LoadRatingAsync();

        }
        catch (Exception ex)
        {
            string ErrorMessage = "Ratings can't load. Try again. " + ex.Message;
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
    }

    [RelayCommand]
    private async Task AddToCart()
    {
        if (SelectedProduct is null) return;

        if (!await ValidateUserAuthenticationAsync()) return;

        var currentUser = await GetCurrentUser();
        if (currentUser is null) return;

        await ExecuteAddToCartAsync(currentUser);
    }

    [RelayCommand]
    private async Task AddRating()
    {
        if (!await ValidateUserAuthenticationAsync()) return;

        var currentUser = await GetCurrentUser();
        if (currentUser is null) return;

        if (await CheckExistingRatingAsync(currentUser)) return;

        await ExecuteAddRatingAsync(currentUser.Id);
    }

    [RelayCommand]
    private async Task SubmitRating()
    {
        if (!await ValidateUserAuthenticationAsync()) return;

        var currentUser = await GetCurrentUser();
        if (currentUser is null) return;

        if (!await CanSubmitRatingAsync(currentUser)) return;

        await ExecuteAddRatingAsync(currentUser.Id);
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
        bool hasServerComment = await _commentService.CheckIfUserHasComment(user.Id, SelectedProduct!.Id);
        if (hasServerComment)
        {
            _dialogService.ShowMessage("You already commented this product before");
            CanAddComment = false;
            return true;
        }

        bool hasLocalComment = Comments.Any(c => c.UserId == user.Id);
        if (hasLocalComment)
        {
            _dialogService.ShowMessage("You already have a comment for this product");
            CanAddComment = false;
            return true;
        }

        return false;
    }

    private async Task<bool> CheckExistingRatingAsync(User user)
    {
        bool hasServerRating = await _commentService.CheckIfUserHasRating(user.Id, SelectedProduct!.Id);
        if (hasServerRating)
        {
            _dialogService.ShowMessage("You already rated this product before");
            return true;
        }

        if (SelectedRating == 0)
        {
            _dialogService.ShowMessage("Please select a rating!");
            return false;
        }

        return false;
    }

    private async Task<bool> CanSubmitRatingAsync(User user)
    {
        if (SelectedProduct == null)
        {
            _dialogService.ShowMessage("No product selected!");
            return false;
        }
        
        bool canSubmit = await _commentService.CheckIfUserHasRating(user.Id, SelectedProduct.Id);
        Console.WriteLine(canSubmit);
        if (canSubmit)
        {
            _dialogService.ShowMessage("You already rated this product before");
            return false;
        }

        if (SelectedRating < 1 || SelectedRating > 5)
        {
            _dialogService.ShowMessage("Please select a valid rating (1-5 stars)");
            return false;
        }

        return true;
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

    private async Task ExecuteAddRatingAsync(string userId)
    {
        await _ratingService.AddRatingAsync(selectedRating: _selectedRating, productId: SelectedProduct.Id,
            userId: userId);

        _dialogService.ShowMessage("Thank you for your rating!");
        SelectedRating = 1;
    }

    private async Task LoadRatingAsync()
    {
        _ratings.Clear();
        await _ratingService.LoadRatingsAsync(SelectedProduct.Id);

        _averageRating = _ratingService.AverageRating;
        Console.WriteLine(_averageRating);
    }
}