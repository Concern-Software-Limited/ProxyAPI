using System.ComponentModel.DataAnnotations;

namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Request model for verifying OTP
/// </summary>
public class VerifyOtpRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
    public string OtpCode { get; set; } = string.Empty;
}
