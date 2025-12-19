using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models;

namespace ProxyAPI.Services;

/// <summary>
/// Service for OTP generation, storage, and validation
/// </summary>
public class OtpService : IOtpService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;

    public OtpService(AppDbContext context, IEmailService emailService, ILogger<OtpService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a 6-digit OTP code
    /// </summary>
    public string GenerateOtpCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    /// <summary>
    /// Generate OTP, save to database, and send via email
    /// </summary>
    public async Task<Otp?> GenerateAndSendOtpAsync(string email)
    {
        try
        {
            // Generate OTP code
            var otpCode = GenerateOtpCode();
            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt.AddMinutes(10);

            // Create OTP entity
            var otp = new Otp
            {
                Email = email,
                OtpCode = otpCode,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                IsUsed = false
            };

            // Save to database
            _context.Otps.Add(otp);
            await _context.SaveChangesAsync();

            // Send OTP via email
            var emailSent = await _emailService.SendOtpEmailAsync(email, otpCode);
            
            if (!emailSent)
            {
                _logger.LogWarning($"OTP generated but email failed to send for {email}");
                // Note: We still return the OTP entity even if email fails
                // You can decide to delete it or handle differently
            }

            _logger.LogInformation($"OTP generated and sent to {email} at {createdAt}");
            return otp;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating OTP for {email}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Validate OTP code for given email
    /// </summary>
    public async Task<bool> ValidateOtpAsync(string email, string otpCode)
    {
        try
        {
            var otp = await _context.Otps
                .Where(o => o.Email == email && o.OtpCode == otpCode && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
            {
                _logger.LogWarning($"Invalid OTP attempt for {email}");
                return false;
            }

            // Check if OTP has expired
            if (otp.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning($"Expired OTP used for {email}");
                return false;
            }

            // Mark OTP as used
            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"OTP validated successfully for {email}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validating OTP for {email}: {ex.Message}");
            return false;
        }
    }
}
