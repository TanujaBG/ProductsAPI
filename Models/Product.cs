namespace ProductsApi.Models;

/// <summary>
/// EF Core entity mapped to the "Products" table. It's a mutable CLASS (not a record) because an
/// entity is a row you load, modify, and save — EF sets its properties and tracks changes to them.
/// </summary>
public class Product
{
    public int Id { get; set; }               // convention: "Id" is the primary key (auto-increment)
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public string? Description { get; set; }

    // Relationship: each Product belongs to ONE Category.
    public int CategoryId { get; set; }       // foreign key (EF infers the relationship from this + the nav)
    public Category? Category { get; set; }    // navigation to the parent category
}
