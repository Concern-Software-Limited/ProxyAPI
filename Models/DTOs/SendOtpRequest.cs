using System.ComponentModel.DataAnnotations;

namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Request model for sending OTP
/// </summary>
public class SendOtpRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
