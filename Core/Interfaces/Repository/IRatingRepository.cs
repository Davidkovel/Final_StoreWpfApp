using Core.Entity;

namespace Core.Repository;

public interface IRatingRepository
{ 
    public Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId);
    public Task<IEnumerable<Rating>> GetRatingsByUserIdAsync(string userId, int productId);
    public Task AddRatingAsync(Rating rating);
    public Task<bool> CheckIfUserHasRatedAsync(string userId, int productId);
}