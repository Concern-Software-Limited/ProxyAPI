namespace ProxyAPI.Models.DTOs;

/// <summary>
/// Response DTO for customer details
/// </summary>
public class UserDetailResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Bot { get; set; } = string.Empty;
    public string Referral { get; set; } = string.Empty;
    public string RechBonus { get; set; } = string.Empty;
}
