namespace MASA.RetailPOS.App.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string? ImageSource { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStockAlert { get; set; }
    public string UnitOfMeasure { get; set; } = "Piece";
    public decimal TaxRate { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
