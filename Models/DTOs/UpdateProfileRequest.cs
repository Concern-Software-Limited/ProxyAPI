using System.ComponentModel.DataAnnotations;

namespace ProxyAPI.Models.DTOs;

/// <summary>
/// DTO for updating user profile
/// </summary>
public class UpdateProfileRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
