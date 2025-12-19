using System.ComponentModel.DataAnnotations;

namespace ProxyAPI.Models.DTOs;

/// <summary>
/// DTO for user registration
/// </summary>
public class RegisterRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional: defaults to "User" if not provided
    /// Can be "Admin" or "User"
    /// </summary>
    public string Role { get; set; } = "User";
}
