using System.Collections.Generic;
using System;

namespace MASA.RetailPOS.App.Models;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = "Completed"; // Completed, Held, Refunded, Cancelled
    public DateTime InvoiceDate { get; set; } = DateTime.Now;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
