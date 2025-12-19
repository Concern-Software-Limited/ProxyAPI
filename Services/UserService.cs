using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models;
using ProxyAPI.Models.DTOs;

namespace ProxyAPI.Services;

/// <summary>
/// Service for user management with password hashing and verification
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    /// <summary>
    /// Register a new user with hashed password
    /// </summary>
    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return null; // Email already in use
        }

        // Create new user
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        // Hash the password
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        // Save to database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Login user with password verification
    /// </summary>
    public async Task<User?> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return null;
        }

        // Verify password
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    public async Task<bool> UpdateUserAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Name = request.Name;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
