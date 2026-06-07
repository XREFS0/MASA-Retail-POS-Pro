using Microsoft.EntityFrameworkCore;
using MASA.RetailPOS.App.Models;

namespace MASA.RetailPOS.App.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=MASA_RetailPOS.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Default Users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", PasswordHash = User.HashPassword("admin123"), Role = "Admin", FullName = "مدير النظام" },
            new User { Id = 2, Username = "manager", PasswordHash = User.HashPassword("manager123"), Role = "Manager", FullName = "مدير الفرع" },
            new User { Id = 3, Username = "cashier", PasswordHash = User.HashPassword("cashier123"), Role = "Cashier", FullName = "كاشير المبيعات" },
            new User { Id = 4, Username = "warehouse", PasswordHash = User.HashPassword("warehouse123"), Role = "Warehouse", FullName = "أمين المخزن" }
        );

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "مشروبات" },
            new Category { Id = 2, Name = "حلويات" },
            new Category { Id = 3, Name = "أطعمة" },
            new Category { Id = 4, Name = "منتجات متنوعة" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "قهوة تركية", Barcode = "1001", SalePrice = 25.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 2, Name = "شاي أخضر", Barcode = "1002", SalePrice = 15.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 3, Name = "عصير برتقال", Barcode = "1003", SalePrice = 20.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 4, Name = "مياه معدنية", Barcode = "1004", SalePrice = 5.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 5, Name = "مشروب غازي", Barcode = "1005", SalePrice = 7.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 6, Name = "كابتشينو", Barcode = "1006", SalePrice = 30.00m, CategoryId = 1, StockQuantity = 100 },
            new Product { Id = 7, Name = "كيك شوكولاتة", Barcode = "1007", SalePrice = 45.00m, CategoryId = 2, StockQuantity = 100 },
            new Product { Id = 8, Name = "كرواسون", Barcode = "1008", SalePrice = 18.00m, CategoryId = 2, StockQuantity = 100 },
            new Product { Id = 9, Name = "ساندويتش الدجاج", Barcode = "1009", SalePrice = 35.00m, CategoryId = 3, StockQuantity = 100 },
            new Product { Id = 10, Name = "بيتزا خضار", Barcode = "1010", SalePrice = 40.00m, CategoryId = 3, StockQuantity = 100 },
            new Product { Id = 11, Name = "بطاطس مقلية", Barcode = "1011", SalePrice = 15.00m, CategoryId = 3, StockQuantity = 100 },
            new Product { Id = 12, Name = "آيس كريم", Barcode = "1012", SalePrice = 25.00m, CategoryId = 2, StockQuantity = 100 }
        );

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "عميل نقدي عام", Phone = "0000000000", LoyaltyPoints = 0, TotalDebt = 0 },
            new Customer { Id = 2, Name = "أحمد محمد", Phone = "01012345678", LoyaltyPoints = 150, TotalDebt = 0 },
            new Customer { Id = 3, Name = "شركة الأمل للتجارة", Phone = "01234567890", LoyaltyPoints = 500, TotalDebt = 1500.50m },
            new Customer { Id = 4, Name = "فاطمة علي", Phone = "01111111111", LoyaltyPoints = 20, TotalDebt = 0 }
        );

        // Seed Suppliers
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "مورد رئيسي", Phone = "01555555555", TotalDebt = 0 },
            new Supplier { Id = 2, Name = "شركة المراعي", Phone = "01000000000", TotalDebt = 5000.00m },
            new Supplier { Id = 3, Name = "مصنع الحلويات الكبرى", Phone = "01222222222", TotalDebt = 12500.00m }
        );
    }
}
