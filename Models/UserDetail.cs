namespace ProxyAPI.Models;

/// <summary>
/// UserDetail entity representing customer details from user_details table
/// </summary>
public class UserDetail
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
