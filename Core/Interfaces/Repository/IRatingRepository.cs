using Core.Entity;

namespace Core.Repository;

public interface IRatingRepository
{ 
    public Task<IEnumerable<Core.Entity.RatingProduct>> GetRatingsByProductIdAsync(int productId);
    public Task<List<int>> GetRatingsByUserIdAsync(string userId, int productId);
    public Task AddRatingAsync(int selectedRating, int productId, string userId);
    public Task<bool> CheckIfUserHasRatedAsync(string userId, int productId);
    Task<bool> HasUserRatedAsync(string userId, int productId);
}