namespace Core.Entity;

public class Cart
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string ProductImageUrl { get; set; } // later for amazon s3
    
    public Cart()
    {
        Id = 0;
        ProductId = 0;
        Quantity = 0;
        ProductName = string.Empty;
        ProductPrice = 0.0m;
        ProductImageUrl = string.Empty;
    }
    
    public Cart(int productId, string userId, int quantity, string productName, decimal productPrice, string productImageUrl)
    {
        ProductId = productId;
        UserId = userId;
        Quantity = quantity;
        ProductName = productName;
        ProductPrice = productPrice;
        ProductImageUrl = productImageUrl;
    }
    
}