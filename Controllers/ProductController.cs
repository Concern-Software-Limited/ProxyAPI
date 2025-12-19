using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProxyAPI.Data;
using ProxyAPI.Models;
using ProxyAPI.Models.DTOs;
using ProxyAPI.Repositories;

namespace ProxyAPI.Controllers;

/// <summary>
/// Product controller for managing products
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDapperRepository _dapperRepo;

    public ProductController(AppDbContext context, IDapperRepository dapperRepo)
    {
        _context = context;
        _dapperRepo = dapperRepo;
    }

    /// <summary>
    /// Get all products with aggregated statistics
    /// GET /api/product
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _context.Products
            .Include(p => p.Variants)
            .Where(p => p.IsActive)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                IsActive = p.IsActive,
                VariantCount = p.Variants!.Count,
                TotalKeys = p.Variants!.Sum(v => v.TotalKeys),
                AvailableKeys = p.Variants!.Sum(v => v.AvailableKeys),
                SoldKeys = p.Variants!.Sum(v => v.SoldKeys),
                CreatedAt = p.CreatedAt
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(ResponseModel<List<ProductResponse>>.SuccessResponse(products, "Products retrieved successfully"));
    }

    /// <summary>
    /// Get product details with variants
    /// GET /api/product/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                IsActive = p.IsActive,
                Variants = p.Variants!.Select(v => new VariantResponse
                {
                    Id = v.Id,
                    ProductId = v.ProductId,
                    ProductName = p.Name,
                    Name = v.Name,
                    TotalKeys = v.TotalKeys,
                    AvailableKeys = v.AvailableKeys,
                    SoldKeys = v.SoldKeys
                }).ToList(),
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Product not found"));
        }

        return Ok(ResponseModel<ProductDetailsResponse>.SuccessResponse(product, "Product retrieved successfully"));
    }

    /// <summary>
    /// Create a new product
    /// POST /api/product
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Image = request.Image,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, 
            ResponseModel<Product>.SuccessResponse(product, "Product created successfully"));
    }

    /// <summary>
    /// Update a product
    /// PUT /api/product/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Invalid input"));
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Product not found"));
        }

        product.Name = request.Name;
        product.Description = request.Description ?? string.Empty;
        product.Image = request.Image;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(ResponseModel<Product>.SuccessResponse(product, "Product updated successfully"));
    }

    /// <summary>
    /// Delete a product
    /// DELETE /api/product/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound(ResponseModel<string>.ErrorResponse("Product not found"));
        }

        // Check if product has variants
        var hasVariants = await _context.Variants.AnyAsync(v => v.ProductId == id);
        if (hasVariants)
        {
            return BadRequest(ResponseModel<string>.ErrorResponse("Cannot delete product with existing variants"));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(ResponseModel<string>.SuccessResponse("Product deleted successfully"));
    }

    /// <summary>
    /// Search products
    /// GET /api/product/search
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string name)
    {
        var products = await _context.Products
            .Include(p => p.Variants)
            .Where(p => p.Name.Contains(name) && p.IsActive)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                IsActive = p.IsActive,
                VariantCount = p.Variants!.Count,
                TotalKeys = p.Variants!.Sum(v => v.TotalKeys),
                AvailableKeys = p.Variants!.Sum(v => v.AvailableKeys),
                SoldKeys = p.Variants!.Sum(v => v.SoldKeys),
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(ResponseModel<List<ProductResponse>>.SuccessResponse(products, "Products searched successfully"));
    }
}
