namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Request model for creating/updating a variant
/// </summary>
public class VariantRequest
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Response model for variant with keys count
/// </summary>
public class VariantResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int AvailableKeys { get; set; }
    public int SoldKeys { get; set; }
}
