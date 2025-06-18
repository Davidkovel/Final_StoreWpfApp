namespace Core.Entity;

public class RatingProduct
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}