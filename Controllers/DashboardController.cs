using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models.DTOs;

namespace ProxyAPI.Controllers;

/// <summary>
/// Dashboard controller for admin dashboard operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all customer details from user_details table
    /// GET /api/dashboard/customers
    /// </summary>
    [HttpGet("customers")]
    public async Task<IActionResult> Customers()
    {
        try
        {
            // Fetch all user details from the user_details table
            var userDetails = await _context.UserDetails
                .Select(ud => new UserDetailResponse
                {
                    Id = ud.Id,
                    FirstName = ud.FirstName,
                    LastName = ud.LastName,
                    Email = ud.Email,
                    CreationDate = ud.CreationDate,
                    Status = ud.Status,
                    Balance = ud.Balance,
                    Bot = ud.Bot,
                    Referral = ud.Referral,
                    RechBonus = ud.RechBonus
                })
                .ToListAsync();

            return Ok(ResponseModel<List<UserDetailResponse>>.SuccessResponse(
                userDetails, 
                "Customers retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<string>.ErrorResponse(
                $"An error occurred while retrieving customers: {ex.Message}"
            ));
        }
    }

    /// <summary>
    /// Get dashboard statistics
    /// GET /api/dashboard/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = new DashboardStatsResponse
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalVariants = await _context.Variants.CountAsync(),
                TotalKeys = await _context.CDKeys.CountAsync(),
                AvailableKeys = await _context.CDKeys.CountAsync(k => k.Status == "Available"),
                SoldKeys = await _context.CDKeys.CountAsync(k => k.Status == "Sold"),
                TotalUsers = await _context.Users.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync()
            };

            return Ok(ResponseModel<DashboardStatsResponse>.SuccessResponse(stats, "Dashboard stats retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<string>.ErrorResponse($"An error occurred: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get products with variants summary for dashboard
    /// GET /api/dashboard/products-summary
    /// </summary>
    [HttpGet("products-summary")]
    public async Task<IActionResult> GetProductsSummary()
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Variants)
                .Select(p => new ProductSummaryResponse
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    VariantCount = p.Variants!.Count,
                    TotalKeys = p.Variants!.Sum(v => v.TotalKeys),
                    AvailableKeys = p.Variants!.Sum(v => v.AvailableKeys),
                    SoldKeys = p.Variants!.Sum(v => v.SoldKeys),
                    Variants = p.Variants!.Select(v => new VariantSummary
                    {
                        VariantId = v.Id,
                        VariantName = v.Name,
                        TotalKeys = v.TotalKeys,
                        AvailableKeys = v.AvailableKeys,
                        SoldKeys = v.SoldKeys
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ResponseModel<List<ProductSummaryResponse>>.SuccessResponse(products, "Products summary retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseModel<string>.ErrorResponse($"An error occurred: {ex.Message}"));
        }
    }
}
