using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Contracts;

/// <summary>Request body for creating/updating a product. The server assigns the Id, so it's not included.</summary>
public record CreateProductRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: StringLength(50, MinimumLength = 1)] string Name,
    [property: Range(0.01, 100_000)] decimal Price,
    [property: Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive id.")] int CategoryId);
