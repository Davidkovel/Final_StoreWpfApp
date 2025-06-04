using Supabase.Gotrue;

namespace Core.Repository;

public interface IAuthRepository
{
    Task<User?> GetCurrentUser();
    Task<User?> Login(string email, string password);
    Task<User?> Register(string email, string password);
    Task Logout();
    bool IsLoggedIn { get; }
}