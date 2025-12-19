namespace ProxyAPI.Models;

/// <summary>
/// Product Variant entity (e.g., 1GB, 2GB, 5GB, etc.)
/// </summary>
public class Variant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "1GB", "2GB"
    public int TotalKeys { get; set; } = 0;
    public int AvailableKeys { get; set; } = 0;
    public int SoldKeys { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Product? Product { get; set; }
    public ICollection<CDKey>? CDKeys { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
