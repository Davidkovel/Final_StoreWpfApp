using Core.Repository;
using Data.DBProvider.SupabaseRemote;
using Supabase.Gotrue;

namespace Data.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly ISupabaseRemoteProvider _provider;

    public AuthRepository(ISupabaseRemoteProvider provider)
    {
        _provider = provider;
    }

    public bool IsLoggedIn => _provider.SupabaseClient.Auth.CurrentSession != null;

    public async Task<User?> Login(string email, string password)
    {
        var response = await _provider.SupabaseClient.Auth.SignIn(email:email, password:password);
        return response?.User;
    }

    public async Task<User?> Register(string email, string password)
    {
        Console.WriteLine(email, password);
        var response = await _provider.SupabaseClient.Auth.SignUp(email: email, password: password);
        return response?.User;
    }

    public async Task Logout()
    {
        await _provider.SupabaseClient.Auth.SignOut();
    }

    public async Task<User?> GetCurrentUser()
    {
        return _provider.SupabaseClient.Auth.CurrentUser;
    }
}