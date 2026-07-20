namespace ProductsApi.Auth;

/// <summary>
/// Dev-only JWT settings used to sign and validate tokens locally (no Entra ID needed).
/// In production these come from configuration / Key Vault, and for Entra ID you'd use an
/// Authority URL instead of a symmetric key.
/// </summary>
public static class JwtSettings
{
    /// <summary>Symmetric signing key (HS256). Must be at least 32 bytes.</summary>
    public const string Key = "dev-only-signing-key-please-change-me-0123456789!";

    /// <summary>Token issuer (the "bank" that mints the token).</summary>
    public const string Issuer = "products-api-dev";

    /// <summary>Token audience (this API).</summary>
    public const string Audience = "products-api";
}
