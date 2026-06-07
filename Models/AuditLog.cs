using System;

namespace MASA.RetailPOS.App.Models;

public class AuditLog : BaseEntity
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
