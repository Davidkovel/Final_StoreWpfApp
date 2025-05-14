namespace Core.Entity;

public class Cart
{
    public int Id { get; set; }

    // public int UserId { get; set; } Temporarily without Auth
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string ProductImageUrl { get; set; } // later for amazon s3
}