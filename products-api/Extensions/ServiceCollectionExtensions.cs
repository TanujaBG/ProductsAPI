using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Auth;
using ProductsApi.Data;

namespace ProductsApi.Extensions;

/// <summary>Registers all application services. Called once from Program.cs.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IHostEnvironment environment)
    {
        // RFC 7807 ProblemDetails for error responses.
        services.AddProblemDetails();

        // Swagger / OpenAPI.
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // CORS - allow the React dev server (Vite) to call this API from the browser.
        services.AddCors(options =>
        {
            options.AddPolicy("frontend", policy =>
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        // EF Core (Scoped) backed by SQLite. For Azure SQL: options.UseSqlServer(connectionString).
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite("Data Source=products.db");

            if (environment.IsDevelopment())
                options.LogTo(Console.WriteLine, LogLevel.Information)
                       .EnableSensitiveDataLogging();
        });

        // JWT bearer authentication. For REAL Entra ID, replace the options body with:
        //   options.Authority = "https://login.microsoftonline.com/<tenant-id>/v2.0";
        //   options.Audience  = "<api-app-id>";
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = JwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = JwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
                    RoleClaimType = "role"
                };
            });

        // Named authorization policies, applied to endpoints via RequireAuthorization("name").
        services.AddAuthorization(options =>
        {
            options.AddPolicy("products.write", policy => policy.RequireClaim("scope", "products.write"));
            options.AddPolicy("admin", policy => policy.RequireRole("admin"));
        });

        return services;
    }
}
