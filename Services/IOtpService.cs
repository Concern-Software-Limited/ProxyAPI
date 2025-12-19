using ProxyAPI.Models;

namespace ProxyAPI.Services;

/// <summary>
/// Interface for OTP management operations
/// </summary>
public interface IOtpService
{
    Task<Otp?> GenerateAndSendOtpAsync(string email);
    Task<bool> ValidateOtpAsync(string email, string otpCode);
    string GenerateOtpCode();
}
