namespace Data.Models;

public class CommentModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string Text { get; set; }
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ProductModel Product { get; set; }

    public CommentModel()
    {
    }
}