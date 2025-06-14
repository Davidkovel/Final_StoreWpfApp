namespace Core.Entity;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } // For amazon s3
    public int CategoryId { get; set; }
    public int Quantity { get; set; }

    public Product()
    {
    }

    // Конструктор с параметрами
    public Product(string name, string description, decimal price,
        string imageUrl, int categoryId, int quantity)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        Quantity = quantity;
    }
}