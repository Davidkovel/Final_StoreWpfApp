namespace Core.Entity;

public class Rating
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    public int RatingValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}