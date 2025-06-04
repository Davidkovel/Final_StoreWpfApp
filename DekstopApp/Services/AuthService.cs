using Core.Repository;
using Supabase.Gotrue;

namespace DekstopApp.Services;

public class AuthService(IAuthRepository authRepository)
{
    public User? CurrentUser { get; private set; }
    public event Action? AuthStateChanged;

    public async Task<User?> GetCurrentUser()
    {
        var user = await authRepository.GetCurrentUser();
        SetUser(user);
        return user;
    }

    public async Task<User?> Login(string email, string password)
    {
        var user = await authRepository.Login(email, password);
        SetUser(user);
        return user;
    }

    public async Task<User?> Register(string email, string password)
    {
        var user = await authRepository.Register(email, password);
        SetUser(user);
        return user;
    }

    public async Task Logout()
    {
        await authRepository.Logout();
        SetUser(null);
    }

    public void SetUser(User? user)
    {
        CurrentUser = user;
        AuthStateChanged?.Invoke();
    }

    public bool IsLoggedIn => CurrentUser != null;
}