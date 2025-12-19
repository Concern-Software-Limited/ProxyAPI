using ProxyAPI.Models;

namespace ProxyAPI.Services;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the authenticated user
    /// </summary>
    string GenerateToken(User user);
}
