namespace Data.Models;

// ProductModel and other models are already DTOs (Data Transfer Objects) that are used to transfer data between layers.

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } // For amazon s3
    public int CategoryId { get; set; }
    public int Quantity { get; set; }
}