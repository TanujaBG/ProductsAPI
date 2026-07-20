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
        //   options.Audience  = "<api-app-id>";   // and delete IssuerSigningKey (Entra publishes its keys).
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Keep the JWT claim names exactly as minted (sub, scope, role) instead of remapping
                // them to long WS-* URIs, so RequireClaim("scope", ...) and RoleClaimType line up.
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
            // Write access: the token must carry the products.write scope.
            options.AddPolicy("products.write", policy => policy.RequireClaim("scope", "products.write"));

            // Delete access: the caller must be in the admin role.
            options.AddPolicy("admin", policy => policy.RequireRole("admin"));
        });

        return services;
    }
}
