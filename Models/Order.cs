namespace ProxyAPI.Models;

/// <summary>
/// Order entity for tracking key purchases
/// </summary>
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int CDKeyId { get; set; }
    public string KeyValue { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Completed"; // Completed, Pending, Cancelled
    
    // Navigation properties
    public User? User { get; set; }
    public Product? Product { get; set; }
    public Variant? Variant { get; set; }
    public CDKey? CDKey { get; set; }
}
