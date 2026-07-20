using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Auth;

namespace ProductsApi.Endpoints;

/// <summary>
/// DEV-ONLY endpoints. The token minter lets you test auth locally without Entra ID.
/// Map these only in Development — NEVER ship them.
/// </summary>
public static class DevEndpoints
{
    public static IEndpointRouteBuilder MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /dev/token[?admin=true]  ->  mint a short-lived JWT signed with the dev key (plays "the bank").
        // Pass ?admin=true to also include a role=admin claim (required by the DELETE endpoint).
        app.MapGet("/dev/token", (bool admin = false) =>
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, "dev-user"),
                new("scope", "products.write")          // satisfies the products.write policy (POST/PUT)
            };
            if (admin)
                claims.Add(new Claim("role", "admin")); // satisfies the admin policy (DELETE)

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: JwtSettings.Issuer,
                audience: JwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials);
            return Results.Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
        })
        .WithTags("Dev");

        return app;
    }
}
