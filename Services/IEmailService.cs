namespace ProxyAPI.Services;

/// <summary>
/// Interface for email sending operations
/// </summary>
public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string toEmail, string otpCode);
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
}
