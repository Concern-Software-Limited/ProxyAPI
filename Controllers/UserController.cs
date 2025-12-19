using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProxyAPI.Models.DTOs;
using ProxyAPI.Services;
using System.Security.Claims;

namespace ProxyAPI.Controllers;

/// <summary>
/// User management controller
/// Demonstrates role-based authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// GET /api/user/all
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        
        var response = users.Select(u => new UserProfileResponse
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        }).ToList();

        return Ok(ResponseModel<List<UserProfileResponse>>.SuccessResponse(response, "Users retrieved successfully"));
    }

    /// <summary>
    /// Delete a user by ID (Admin only)
    /// DELETE /api/user/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("User not found"));
        }

        return Ok(ResponseModel<string>.SuccessResponse("User deleted successfully"));
    }

    /// <summary>
    /// Update own profile (User and Admin)
    /// PUT /api/user/profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        // Get current user ID from token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ResponseModel<string>.ErrorResponse("Invalid token"));
        }

        var result = await _userService.UpdateUserAsync(userId, request);
        if (!result)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("User not found"));
        }

        return Ok(ResponseModel<string>.SuccessResponse("Profile updated successfully"));
    }
}
