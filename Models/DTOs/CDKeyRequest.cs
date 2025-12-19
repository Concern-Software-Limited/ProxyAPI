namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Request model for adding multiple CD keys
/// </summary>
public class AddCDKeysRequest
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public List<string> Keys { get; set; } = new();
    public string Status { get; set; } = "Available";
}

/// <summary>
/// Response model for CD key with details
/// </summary>
public class CDKeyResponse
{
    public int Id { get; set; }
    public string KeyValue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; }
    public DateTime? SoldDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
}

/// <summary>
/// Request model for filtering CD keys
/// </summary>
public class CDKeyFilterRequest
{
    public int? ProductId { get; set; }
    public int? VariantId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string SortBy { get; set; } = "Newest"; // Newest, Oldest
}
