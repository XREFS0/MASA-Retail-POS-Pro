using System.Collections.Generic;

namespace MASA.RetailPOS.App.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconSource { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
