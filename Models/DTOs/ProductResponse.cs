namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Product response with aggregated statistics
/// </summary>
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsActive { get; set; }
    public int VariantCount { get; set; }
    public int TotalKeys { get; set; }
    public int AvailableKeys { get; set; }
    public int SoldKeys { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Product details with variants
/// </summary>
public class ProductDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsActive { get; set; }
    public List<VariantResponse> Variants { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model for creating/updating a product
/// </summary>
public class ProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; } = true;
}
