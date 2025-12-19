namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Dashboard statistics response
/// </summary>
public class DashboardStatsResponse
{
    public int TotalProducts { get; set; }
    public int TotalVariants { get; set; }
    public int TotalKeys { get; set; }
    public int AvailableKeys { get; set; }
    public int SoldKeys { get; set; }
    public int TotalUsers { get; set; }
    public int TotalOrders { get; set; }
}

/// <summary>
/// Product summary for dashboard
/// </summary>
public class ProductSummaryResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int VariantCount { get; set; }
    public int TotalKeys { get; set; }
    public int AvailableKeys { get; set; }
    public int SoldKeys { get; set; }
    public List<VariantSummary> Variants { get; set; } = new();
}

public class VariantSummary
{
    public int VariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public int TotalKeys { get; set; }
    public int AvailableKeys { get; set; }
    public int SoldKeys { get; set; }
}
