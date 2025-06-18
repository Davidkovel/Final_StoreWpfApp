namespace Data.Database.Commands.Rating;

public interface IRatingSqlCommandProvider
{
    public string GetRatingsByProductIdAsync(int productId);
    public string GetRatingsByUserIdAsync(string userId, int productId);
    public string AddRatingAsync(int selectedRating, int productId, string userId);
    public string CheckIfUserHasRatedAsync(string userId, int productId);
    
}