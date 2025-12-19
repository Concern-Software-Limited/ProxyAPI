namespace ProxyAPI.Models;

/// <summary>
/// CD Key entity for product variants
/// </summary>
public class CDKey
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public string KeyValue { get; set; } = string.Empty; // The actual key (e.g., "xyz-abc-a12")
    public string Status { get; set; } = "Available"; // Available, Sold
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SoldDate { get; set; }
    public int? UserId { get; set; } // User who purchased the key
    
    // Navigation properties
    public Product? Product { get; set; }
    public Variant? Variant { get; set; }
    public User? User { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
