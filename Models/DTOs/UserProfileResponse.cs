namespace ProxyAPI.Models.DTOs;

/// <summary>
/// User profile response (without sensitive data)
/// </summary>
public class UserProfileResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
