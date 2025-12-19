namespace ProxyAPI.Models;

/// <summary>
/// User entity representing users in the system
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Role can be: "Admin" or "User"
    /// </summary>
    public string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<CDKey>? CDKeys { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
