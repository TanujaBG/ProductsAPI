using Microsoft.EntityFrameworkCore;
using ProductsApi.Contracts;
using ProductsApi.Data;
using ProductsApi.Filters;
using ProductsApi.Models;

namespace ProductsApi.Endpoints;

/// <summary>
/// Maps the product/category endpoints, grouped by API version with MapGroup (URL-path versioning).
/// Each handler injects AppDbContext (Scoped: one per request) and uses async EF Core calls.
/// </summary>
public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        // ===== API v1 =====
        var v1 = app.MapGroup("/v1").WithTags("Products v1");

        // GET /v1/products  ->  list all (200).
        v1.MapGet("/products", async (AppDbContext db) =>
            Results.Ok(await db.Products.ToListAsync()));

        // GET /v1/products/{id}  ->  one (200) or 404.
        v1.MapGet("/products/{id:int}", async (int id, AppDbContext db) =>
        {
            Product? product = await db.Products.FindAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        // POST /v1/products  ->  create (201), or 400 if the category is missing/invalid.
        // Requires the products.write scope (authorization policy).
        v1.MapPost("/products", async (CreateProductRequest request, AppDbContext db) =>
        {
            if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["CategoryId"] = new[] { $"Category {request.CategoryId} does not exist." }
                });

            var product = new Product { Name = request.Name, Price = request.Price, CategoryId = request.CategoryId };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/v1/products/{product.Id}", product);
        })
        .AddEndpointFilter<ValidationFilter>()
        .RequireAuthorization("products.write");

        // PUT /v1/products/{id}  ->  replace (204) or 404. Requires the products.write scope.
        v1.MapPut("/products/{id:int}", async (int id, CreateProductRequest request, AppDbContext db) =>
        {
            Product? product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["CategoryId"] = new[] { $"Category {request.CategoryId} does not exist." }
                });

            product.Name = request.Name;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            await db.SaveChangesAsync();      // change tracking turns the edits into an UPDATE
            return Results.NoContent();
        })
        .AddEndpointFilter<ValidationFilter>()
        .RequireAuthorization("products.write");

        // DELETE /v1/products/{id}  ->  remove (204) or 404. Admin role only.
        v1.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
        {
            Product? product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .RequireAuthorization("admin");

        // GET /v1/products/{id}/with-category  ->  project the related category name (no JSON cycle).
        v1.MapGet("/products/{id:int}/with-category", async (int id, AppDbContext db) =>
        {
            var result = await db.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new { p.Id, p.Name, p.Price, Category = p.Category!.Name })
                .FirstOrDefaultAsync();
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        // GET /v1/categories  ->  each category with its product count (reverse navigation).
        v1.MapGet("/categories", async (AppDbContext db) =>
            Results.Ok(await db.Categories
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, ProductCount = c.Products.Count })
                .ToListAsync()));

        // GET /v1/products/page?page=1&size=2  ->  pagination (Skip/Take -> SQL OFFSET/LIMIT).
        v1.MapGet("/products/page", async (int page, int size, AppDbContext db) =>
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            List<Product> items = await db.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)              // pagination needs a stable ORDER BY
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return Results.Ok(items);
        });

        // ===== API v2 (richer response) =====
        var v2 = app.MapGroup("/v2").WithTags("Products v2");

        // GET /v2/products  ->  adds a formatted DisplayPrice field (a shape change => a new version).
        v2.MapGet("/products", async (AppDbContext db) =>
        {
            List<Product> items = await db.Products.AsNoTracking().ToListAsync();
            return Results.Ok(items.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                DisplayPrice = $"${p.Price:0.00}"
            }));
        });

        return app;
    }
}
