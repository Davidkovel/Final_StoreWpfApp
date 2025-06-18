namespace Data.DTOs;

public record RatingTask(
    int Rating,
    int ProductId,
    string UserId
);