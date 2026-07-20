namespace ProductsApi.Models;

/// <summary>A product category. One category has many products (one-to-many).</summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Reverse navigation: lets you go from a Category to all its Products.
    public List<Product> Products { get; set; } = new();
}
