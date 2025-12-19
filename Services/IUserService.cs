using ProxyAPI.Models;
using ProxyAPI.Models.DTOs;

namespace ProxyAPI.Services;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserService
{
    Task<User?> RegisterAsync(RegisterRequest request);
    Task<User?> LoginAsync(LoginRequest request);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int userId, UpdateProfileRequest request);
    Task<bool> DeleteUserAsync(int userId);
}
