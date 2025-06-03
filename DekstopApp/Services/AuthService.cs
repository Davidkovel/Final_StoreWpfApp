using Core.Repository;
using Supabase.Gotrue;

namespace DekstopApp.Services;

public class AuthService(IAuthRepository authRepository)
{
    public async Task<User?> GetCurrentUser()
    {
        return await authRepository.GetCurrentUser();
    }

    public async Task<User?> Login(string email, string password)
    {
        return await authRepository.Login(email, password);
    }

    public async Task<User?> Register(string email, string password)
    {
        return await authRepository.Register(email, password);
    }

    public async Task Logout()
    {
        await authRepository.Logout();
    }

    public bool IsLoggedIn => authRepository.IsLoggedIn;
}