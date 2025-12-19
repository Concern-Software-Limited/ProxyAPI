namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Response model for OTP sending
/// </summary>
public class SendOtpResponse
{
    public string Email { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
