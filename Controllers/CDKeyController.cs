using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models;
using ProxyAPI.Models.DTOs;

namespace ProxyAPI.Controllers;

/// <summary>
/// CD Key controller for managing product keys
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CDKeyController : ControllerBase
{
    private readonly AppDbContext _context;

    public CDKeyController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all CD keys with filters
    /// GET /api/cdkey
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllKeys([FromQuery] CDKeyFilterRequest filter)
    {
        var query = _context.CDKeys
            .Include(k => k.Product)
            .Include(k => k.Variant)
            .Include(k => k.User)
            .AsQueryable();

        // Apply filters
        if (filter.ProductId.HasValue)
        {
            query = query.Where(k => k.ProductId == filter.ProductId.Value);
        }

        if (filter.VariantId.HasValue)
        {
            query = query.Where(k => k.VariantId == filter.VariantId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            query = query.Where(k => k.Status == filter.Status);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(k => k.AddedDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(k => k.AddedDate <= filter.EndDate.Value);
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() == "oldest"
            ? query.OrderBy(k => k.AddedDate)
            : query.OrderByDescending(k => k.AddedDate);

        var keys = await query
            .Select(k => new CDKeyResponse
            {
                Id = k.Id,
                KeyValue = k.KeyValue,
                Status = k.Status,
                AddedDate = k.AddedDate,
                SoldDate = k.SoldDate,
                ProductName = k.Product!.Name,
                VariantName = k.Variant!.Name,
                UserEmail = k.User != null ? k.User.Email : null
            })
            .ToListAsync();

        return Ok(ResponseModel<List<CDKeyResponse>>.SuccessResponse(keys, "Keys retrieved successfully"));
    }

    /// <summary>
    /// Get CD keys by product and variant
    /// GET /api/cdkey/product/{productId}/variant/{variantId}
    /// </summary>
    [HttpGet("product/{productId}/variant/{variantId}")]
    public async Task<IActionResult> GetKeysByProductAndVariant(int productId, int variantId, [FromQuery] string? status = null)
    {
        var query = _context.CDKeys
            .Include(k => k.Product)
            .Include(k => k.Variant)
            .Include(k => k.User)
            .Where(k => k.ProductId == productId && k.VariantId == variantId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(k => k.Status == status);
        }

        var keys = await query
            .Select(k => new CDKeyResponse
            {
                Id = k.Id,
                KeyValue = k.KeyValue,
                Status = k.Status,
                AddedDate = k.AddedDate,
                SoldDate = k.SoldDate,
                ProductName = k.Product!.Name,
                VariantName = k.Variant!.Name,
                UserEmail = k.User != null ? k.User.Email : null
            })
            .OrderByDescending(k => k.AddedDate)
            .ToListAsync();

        return Ok(ResponseModel<List<CDKeyResponse>>.SuccessResponse(keys, "Keys retrieved successfully"));
    }

    /// <summary>
    /// Add multiple CD keys
    /// POST /api/cdkey
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddKeys([FromBody] AddCDKeysRequest request)
    {
        if (!ModelState.IsValid || request.Keys == null || request.Keys.Count == 0)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input or no keys provided"));
        }

        // Validate product and variant exist
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Product not found"));
        }

        var variant = await _context.Variants.FindAsync(request.VariantId);
        if (variant == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Variant not found"));
        }

        // Check for duplicate keys
        var existingKeys = await _context.CDKeys
            .Where(k => request.Keys.Contains(k.KeyValue))
            .Select(k => k.KeyValue)
            .ToListAsync();

        if (existingKeys.Any())
        {
            return BadRequest(ResponseModel<string>.ErrorResponse($"Duplicate keys found: {string.Join(", ", existingKeys)}"));
        }

        // Add keys
        var cdKeys = request.Keys.Select(key => new CDKey
        {
            ProductId = request.ProductId,
            VariantId = request.VariantId,
            KeyValue = key.Trim(),
            Status = request.Status,
            AddedDate = DateTime.UtcNow
        }).ToList();

        _context.CDKeys.AddRange(cdKeys);

        // Update variant statistics
        variant.TotalKeys += cdKeys.Count;
        if (request.Status == "Available")
        {
            variant.AvailableKeys += cdKeys.Count;
        }

        await _context.SaveChangesAsync();

        return Ok(ResponseModel<string>.SuccessResponse($"{cdKeys.Count} keys added successfully"));
    }

    /// <summary>
    /// Delete a CD key
    /// DELETE /api/cdkey/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKey(int id)
    {
        var key = await _context.CDKeys.FindAsync(id);
        if (key == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Key not found"));
        }

        if (key.Status == "Sold")
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Cannot delete sold keys"));
        }

        // Update variant statistics
        var variant = await _context.Variants.FindAsync(key.VariantId);
        if (variant != null)
        {
            variant.TotalKeys--;
            if (key.Status == "Available")
            {
                variant.AvailableKeys--;
            }
        }

        _context.CDKeys.Remove(key);
        await _context.SaveChangesAsync();

        return Ok(ResponseModel<string>.SuccessResponse("Key deleted successfully"));
    }

    /// <summary>
    /// Get sold keys
    /// GET /api/cdkey/sold
    /// </summary>
    [HttpGet("sold")]
    public async Task<IActionResult> GetSoldKeys([FromQuery] SoldKeyFilterRequest filter)
    {
        var query = _context.CDKeys
            .Include(k => k.Product)
            .Include(k => k.Variant)
            .Include(k => k.User)
            .Where(k => k.Status == "Sold");

        // Apply search filter
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(k =>
                k.KeyValue.Contains(filter.Search) ||
                k.Product!.Name.Contains(filter.Search) ||
                k.User!.Email.Contains(filter.Search));
        }

        // Apply date filters
        if (filter.StartDate.HasValue)
        {
            query = query.Where(k => k.SoldDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(k => k.SoldDate <= filter.EndDate.Value);
        }

        var soldKeys = await query
            .Select(k => new SoldKeyResponse
            {
                KeyValue = k.KeyValue,
                ProductName = k.Product!.Name,
                VariantName = k.Variant!.Name,
                UserEmail = k.User!.Email,
                SoldDate = k.SoldDate!.Value
            })
            .OrderByDescending(k => k.SoldDate)
            .ToListAsync();

        return Ok(ResponseModel<List<SoldKeyResponse>>.SuccessResponse(soldKeys, "Sold keys retrieved successfully"));
    }
}
