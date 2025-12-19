namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Response model for sold keys list
/// </summary>
public class SoldKeyResponse
{
    public string KeyValue { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime SoldDate { get; set; }
}

/// <summary>
/// Request model for filtering sold keys
/// </summary>
public class SoldKeyFilterRequest
{
    public string? Search { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
