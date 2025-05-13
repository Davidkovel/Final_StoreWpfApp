namespace Core.Entity;

public class Cart
{
    public int Id { get; set; }
    // public int UserId { get; set; } Temporarily withoud Auth
    public List<Product> Products { get; set; } = new List<Product>();
    
    
    public decimal TotalPrice => Products.Sum(p => p.Price * p.Quantity);
}