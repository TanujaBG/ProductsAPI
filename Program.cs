using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Endpoints;
using ProductsApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// All service registration lives in AddApiServices (Extensions/ServiceCollectionExtensions.cs).
builder.Services.AddApiServices(builder.Environment);

var app = builder.Build();

// Apply pending EF Core migrations at startup so the database exists and is current.
// (Fine for dev/learning; in production you would usually migrate in a deploy step, not at app start.)
using (IServiceScope scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// Swagger + the dev-only token endpoint are Development-only.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapDevEndpoints();
}

// Auth middleware: identity (authN) THEN access (authZ). Order matters.
app.UseAuthentication();
app.UseAuthorization();

// All product/category endpoints (v1 + v2) live in Endpoints/ProductEndpoints.cs.
app.MapProductEndpoints();

app.Run();
