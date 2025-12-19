using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models;
using ProxyAPI.Models.DTOs;

namespace ProxyAPI.Controllers;

/// <summary>
/// Variant controller for managing product variants
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class VariantController : ControllerBase
{
    private readonly AppDbContext _context;

    public VariantController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all variants
    /// GET /api/variant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllVariants()
    {
        var variants = await _context.Variants
            .Include(v => v.Product)
            .Select(v => new VariantResponse
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product!.Name,
                Name = v.Name,
                TotalKeys = v.TotalKeys,
                AvailableKeys = v.AvailableKeys,
                SoldKeys = v.SoldKeys
            })
            .ToListAsync();

        return Ok(ResponseModel<List<VariantResponse>>.SuccessResponse(variants, "Variants retrieved successfully"));
    }

    /// <summary>
    /// Get variants by product ID
    /// GET /api/variant/product/{productId}
    /// </summary>
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetVariantsByProduct(int productId)
    {
        var variants = await _context.Variants
            .Where(v => v.ProductId == productId)
            .Select(v => new VariantResponse
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product!.Name,
                Name = v.Name,
                TotalKeys = v.TotalKeys,
                AvailableKeys = v.AvailableKeys,
                SoldKeys = v.SoldKeys
            })
            .ToListAsync();

        return Ok(ResponseModel<List<VariantResponse>>.SuccessResponse(variants, "Variants retrieved successfully"));
    }

    /// <summary>
    /// Get variant by ID
    /// GET /api/variant/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVariantById(int id)
    {
        var variant = await _context.Variants
            .Include(v => v.Product)
            .Where(v => v.Id == id)
            .Select(v => new VariantResponse
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product!.Name,
                Name = v.Name,
                TotalKeys = v.TotalKeys,
                AvailableKeys = v.AvailableKeys,
                SoldKeys = v.SoldKeys
            })
            .FirstOrDefaultAsync();

        if (variant == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Variant not found"));
        }

        return Ok(ResponseModel<VariantResponse>.SuccessResponse(variant, "Variant retrieved successfully"));
    }

    /// <summary>
    /// Create a new variant
    /// POST /api/variant
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVariant([FromBody] VariantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        // Check if product exists
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId);
        if (!productExists)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Product not found"));
        }

        var variant = new Variant
        {
            ProductId = request.ProductId,
            Name = request.Name,
            TotalKeys = 0,
            AvailableKeys = 0,
            SoldKeys = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Variants.Add(variant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVariantById), new { id = variant.Id },
            ResponseModel<Variant>.SuccessResponse(variant, "Variant created successfully"));
    }

    /// <summary>
    /// Update a variant
    /// PUT /api/variant/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVariant(int id, [FromBody] VariantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var variant = await _context.Variants.FindAsync(id);
        if (variant == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Variant not found"));
        }

        variant.Name = request.Name;
        variant.ProductId = request.ProductId;

        await _context.SaveChangesAsync();

        return Ok(ResponseModel<Variant>.SuccessResponse(variant, "Variant updated successfully"));
    }

    /// <summary>
    /// Delete a variant
    /// DELETE /api/variant/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVariant(int id)
    {
        var variant = await _context.Variants.FindAsync(id);
        if (variant == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Variant not found"));
        }

        // Check if variant has keys
        var hasKeys = await _context.CDKeys.AnyAsync(k => k.VariantId == id);
        if (hasKeys)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Cannot delete variant with existing keys"));
        }

        _context.Variants.Remove(variant);
        await _context.SaveChangesAsync();

        return Ok(ResponseModel<string>.SuccessResponse("Variant deleted successfully"));
    }
}
