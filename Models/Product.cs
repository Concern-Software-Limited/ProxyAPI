namespace ProxyAPI.Models;

/// <summary>
/// Product entity
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Legacy properties (keep for compatibility)
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    // Navigation properties
    public ICollection<Variant>? Variants { get; set; }
    public ICollection<CDKey>? CDKeys { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
