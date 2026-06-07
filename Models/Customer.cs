namespace MASA.RetailPOS.App.Models;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public decimal TotalDebt { get; set; }
    public int LoyaltyPoints { get; set; }
}
