using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DekstopApp.Services;
using Microsoft.Extensions.Logging;
using Supabase.Gotrue;

namespace DekstopApp.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    [ObservableProperty] protected string email = string.Empty;
    [ObservableProperty] protected string password = String.Empty;

    private readonly ILogger<AuthViewModel> _logger;
    private readonly AuthService _authService;

    public AuthViewModel(ILogger<AuthViewModel> logger, AuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }


    public async Task<User?> GetCurrentUser()
    {
        try
        {
            return await _authService.GetCurrentUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current user");
            return null;
        }
    }

//    [RelayCommand]
    public async Task<User?> Login(string emailFromUser, string passwordFromUser)
    {
        try
        {
            var user = await _authService.Login(emailFromUser, passwordFromUser);

            if (user == null)
            {
                _logger.LogWarning("Login failed for user {Email}", email);
                return null;
            }

            _logger.LogInformation("User {Email} logged in successfully", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to login");
            return null;
        }
    }

    // [RelayCommand]
    public async Task<User?> Register(string emailFromUser, string passwordFromUser)
    {
        try
        {
            var user = await _authService.Register(emailFromUser, passwordFromUser);

            if (user == null)
            {
                _logger.LogWarning("Registration failed for user {Email}", email);
                return null;
            }

            _logger.LogInformation("User {Email} registered successfully", email);
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _logger.LogError(ex, "Failed to register");
            return null;
        }
    }

    public async Task Logout()
    {
        try
        {
            await _authService.Logout();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to logout");
        }
    }

    public bool IsLoggedIn => _authService.IsLoggedIn;
}