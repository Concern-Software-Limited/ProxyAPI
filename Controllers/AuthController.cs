using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProxyAPI.Models.DTOs;
using ProxyAPI.Services;
using System.Security.Claims;

namespace ProxyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IOtpService _otpService;

    public AuthController(IUserService userService, ITokenService tokenService, IOtpService otpService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _otpService = otpService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var user = await _userService.RegisterAsync(request);
        if (user == null)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Email already exists"));
        }

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Ok(ResponseModel<UserProfileResponse>.SuccessResponse(response, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var user = await _userService.LoginAsync(request);
        if (user == null)
        {
            return Unauthorized(ResponseModel<string>.ErrorResponse("Invalid email or password"));
        }

        // Generate JWT token
        var token = _tokenService.GenerateToken(user);

        var response = new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        return Ok(ResponseModel<LoginResponse>.SuccessResponse(response, "Login successful"));
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        // Get user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ResponseModel<string>.ErrorResponse("Invalid token"));
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("User not found"));
        }

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Ok(ResponseModel<UserProfileResponse>.SuccessResponse(response, "Profile retrieved successfully"));
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var otp = await _otpService.GenerateAndSendOtpAsync(request.Email);
        if (otp == null)
        {
            return StatusCode(500, ResponseModel<string>.ErrorResponse("Failed to generate or send OTP"));
        }

        var response = new SendOtpResponse
        {
            Email = otp.Email,
            SentAt = otp.CreatedAt,
            ExpiresAt = otp.ExpiresAt
        };

        return Ok(ResponseModel<SendOtpResponse>.SuccessResponse(response, "OTP sent successfully to your email"));
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var isValid = await _otpService.ValidateOtpAsync(request.Email, request.OtpCode);
        if (!isValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid or expired OTP"));
        }

        return Ok(ResponseModel<string>.SuccessResponse("OTP verified successfully", "OTP verified successfully"));
    }
}
