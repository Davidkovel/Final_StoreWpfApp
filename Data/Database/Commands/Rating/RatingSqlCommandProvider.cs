namespace Data.Database.Commands.Rating;

public class RatingCommandProvider : IRatingSqlCommandProvider
{
    private IRatingSqlCommandProvider _ratingSqlCommandProviderImplementation;

    public string GetRatingsByProductIdAsync(int productId) => @"
        SELECT * FROM Ratings WHERE ProductId = @ProductId
    ";

    public string GetRatingsByUserIdAsync(string userId, int productId) => @"
        SELECT * FROM Ratings WHERE UserId = @UserId AND ProductId = @ProductId
        ";

    public string AddRatingAsync(int selectedRating, int productId, string userId) => @"
        INSERT INTO Ratings (UserId, ProductId, Rating)
        VALUES (@UserId, @ProductId, @Rating)
    ";

    public string CheckIfUserHasRatedAsync(string userId, int productId) => @"
        SELECT COUNT(*) FROM Ratings WHERE UserId = @UserId AND ProductId = @ProductId
    ";
}