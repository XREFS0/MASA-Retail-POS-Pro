using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using MASA.RetailPOS.App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MASA.RetailPOS.App.ViewModels;

public partial class PosViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;
    private readonly System.IServiceProvider _serviceProvider;
    private System.Collections.Generic.List<Product> _allProductsCache = new();

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private ObservableCollection<InvoiceItem> _cartItems = new();

    [ObservableProperty]
    private bool _isDiscountVisible;

    [ObservableProperty]
    private decimal _discountAmount;

    [ObservableProperty]
    private bool _isAddCustomerVisible;

    [ObservableProperty]
    private string _newCustomerName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    partial void OnSearchQueryChanged(string value)
    {
        FilterProducts();
    }

    partial void OnDiscountAmountChanged(decimal value)
    {
        UpdateTotals();
    }

    public PosViewModel(ApplicationDbContext context, System.IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // Load Categories
        var categories = await _context.Categories.ToListAsync();
        Categories = new ObservableCollection<Category>(categories);
        Categories.Insert(0, new Category { Id = 0, Name = "الكل" });

        // Load Customers
        var customers = await _context.Customers.ToListAsync();
        Customers = new ObservableCollection<Customer>(customers);
        SelectedCustomer = Customers.FirstOrDefault(c => c.Name.Contains("عام"));

        // Load Products
        _allProductsCache = await _context.Products.ToListAsync();
        FilterProducts();
    }

    private void FilterProducts()
    {
        var filtered = _allProductsCache.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var lowerQuery = SearchQuery.ToLower();
            filtered = filtered.Where(p => 
                (p.Name != null && p.Name.ToLower().Contains(lowerQuery)) || 
                (p.Barcode != null && p.Barcode.Contains(lowerQuery)));
        }

        Products = new ObservableCollection<Product>(filtered);
    }

    [RelayCommand]
    private void SelectCategory(Category category)
    {
        if (category == null) return;

        SearchQuery = string.Empty; // Clear search on category change

        if (category.Id == 0) // "All" category
        {
            Products = new ObservableCollection<Product>(_allProductsCache);
        }
        else
        {
            Products = new ObservableCollection<Product>(_allProductsCache.Where(p => p.CategoryId == category.Id));
        }
    }

    [RelayCommand]
    private void AddToCart(Product product)
    {
        if (product == null) return;
        
        if (product.StockQuantity <= 0)
        {
            MessageBox.Show($"المنتج ({product.Name}) غير متوفر في المخزون حالياً", "نفاد الكمية", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var existingItem = CartItems.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            if (existingItem.Quantity >= product.StockQuantity)
            {
                MessageBox.Show($"الكمية المطلوبة غير متوفرة. المتاح: {product.StockQuantity}", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            existingItem.Quantity++;
            OnPropertyChanged(nameof(CartItems));
        }
        else
        {
            CartItems.Add(new InvoiceItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.SalePrice,
                Quantity = 1
            });
        }
        UpdateTotals();
    }

    [RelayCommand]
    private void IncreaseQuantity(InvoiceItem item)
    {
        if (item != null)
        {
            var product = _allProductsCache.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null && item.Quantity >= product.StockQuantity)
            {
                MessageBox.Show($"الكمية المطلوبة غير متوفرة. المتاح: {product.StockQuantity}", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            item.Quantity++;
            OnPropertyChanged(nameof(CartItems));
            UpdateTotals();
        }
    }

    [RelayCommand]
    private void DecreaseQuantity(InvoiceItem item)
    {
        if (item != null)
        {
            item.Quantity--;
            if (item.Quantity <= 0)
            {
                CartItems.Remove(item);
            }
            OnPropertyChanged(nameof(CartItems));
            UpdateTotals();
        }
    }

    [RelayCommand]
    private void ClearCart()
    {
        CartItems.Clear();
        UpdateTotals();
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (!CartItems.Any())
        {
            MessageBox.Show("سلة المشتريات فارغة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var mainViewModel = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainViewModel>(_serviceProvider);
        var currentUser = mainViewModel.CurrentUser;
        
        if (currentUser == null) return;

        // Create Invoice
        var invoice = new Invoice
        {
            InvoiceNumber = $"INV-{System.DateTime.Now:yyyyMMddHHmmss}",
            Subtotal = this.Subtotal,
            TaxTotal = this.Tax,
            Discount = this.Discount,
            GrandTotal = this.GrandTotal,
            Status = "Completed",
            InvoiceDate = System.DateTime.Now,
            UserId = currentUser.Id,
            CustomerId = SelectedCustomer?.Id
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(); // Save to generate Invoice ID

        // Add Items and deduct stock
        foreach (var item in CartItems)
        {
            item.InvoiceId = invoice.Id;
            _context.InvoiceItems.Add(item);

            var dbProduct = await _context.Products.FindAsync(item.ProductId);
            if (dbProduct != null)
            {
                dbProduct.StockQuantity -= item.Quantity;
            }
        }

        // Add Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            UserId = currentUser.Id,
            Username = currentUser.Username,
            Action = "إصدار فاتورة",
            Details = $"تم إصدار فاتورة مبيعات برقم {invoice.InvoiceNumber} بقيمة {invoice.GrandTotal:C}"
        });

        await _context.SaveChangesAsync();

        // Save to PDF file
        string invoicesFolderPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "الفواتير");
        if (!System.IO.Directory.Exists(invoicesFolderPath))
        {
            System.IO.Directory.CreateDirectory(invoicesFolderPath);
        }

        string invoiceFileName = System.IO.Path.Combine(invoicesFolderPath, $"{invoice.InvoiceNumber}.pdf");
        
        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));
                page.ContentFromRightToLeft();

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });

            void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
            {
                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("MASA Retail POS").FontSize(24).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                        column.Item().Text($"فاتورة مبيعات ضريبية").FontSize(16).SemiBold();
                        column.Item().PaddingTop(5).Text($"رقم الفاتورة: {invoice.InvoiceNumber}");
                        column.Item().Text($"التاريخ: {invoice.InvoiceDate:yyyy-MM-dd hh:mm tt}");
                        column.Item().Text($"العميل: {(SelectedCustomer != null ? SelectedCustomer.Name : "عميل عام")}");
                        column.Item().Text($"الكاشير: {currentUser.Username}");
                    });
                });
            }

            void ComposeContent(QuestPDF.Infrastructure.IContainer container)
            {
                container.PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Product Name
                            columns.RelativeColumn(1); // Quantity
                            columns.RelativeColumn(1); // Unit Price
                            columns.RelativeColumn(1); // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(2).Text("المنتج").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("الكمية").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("سعر الوحدة").SemiBold();
                            header.Cell().BorderBottom(1).Padding(2).Text("الإجمالي").SemiBold();
                        });

                        foreach (var item in CartItems)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(2).Text(item.ProductName);
                            table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(2).Text(item.Quantity.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(2).Text($"{item.UnitPrice:N2}");
                            table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(2).Text($"{item.Subtotal:N2}");
                        }
                    });

                    column.Item().PaddingTop(10).AlignRight().Column(c =>
                    {
                        c.Item().Text($"الإجمالي قبل الضريبة: {this.Subtotal:N2}").SemiBold();
                        c.Item().Text($"الخصم المطبق: {this.Discount:N2}").SemiBold();
                        c.Item().Text($"الضريبة المضافة (14%): {this.Tax:N2}").SemiBold();
                        c.Item().PaddingTop(5).Text($"الإجمالي النهائي للدفع: {this.GrandTotal:N2}").FontSize(16).Bold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                    });
                });
            }

            void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
            {
                container.AlignCenter().Text("شكراً لتسوقكم معنا! - MASA Retail POS").FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
            }
        })
        .GeneratePdf(invoiceFileName);

        MessageBox.Show($"تم إصدار الفاتورة بنجاح!\nرقم الفاتورة: {invoice.InvoiceNumber}\nالإجمالي: {invoice.GrandTotal:N2}\n\n(تم حفظ الفاتورة بصيغة PDF في مجلد 'الفواتير')", "عملية ناجحة", MessageBoxButton.OK, MessageBoxImage.Information);
        
        ClearCart();
        DiscountAmount = 0;
        await LoadDataAsync(); // Reload products to get fresh stock
    }

    [RelayCommand]
    private async Task HoldInvoiceAsync()
    {
        if (!CartItems.Any())
        {
            MessageBox.Show("سلة المشتريات فارغة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var mainViewModel = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainViewModel>(_serviceProvider);
        var currentUser = mainViewModel.CurrentUser;
        if (currentUser == null) return;

        var invoice = new Invoice
        {
            InvoiceNumber = $"HLD-{System.DateTime.Now:yyyyMMddHHmmss}",
            Subtotal = this.Subtotal,
            TaxTotal = this.Tax,
            Discount = this.Discount,
            GrandTotal = this.GrandTotal,
            Status = "Held",
            InvoiceDate = System.DateTime.Now,
            UserId = currentUser.Id,
            CustomerId = SelectedCustomer?.Id
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        foreach (var item in CartItems)
        {
            item.InvoiceId = invoice.Id;
            _context.InvoiceItems.Add(item);
        }
        await _context.SaveChangesAsync();

        MessageBox.Show($"تم تعليق الفاتورة بنجاح!\nرقم المرجع: {invoice.InvoiceNumber}", "تعليق فاتورة", MessageBoxButton.OK, MessageBoxImage.Information);
        
        ClearCart();
        DiscountAmount = 0;
    }

    [RelayCommand]
    private void ToggleDiscount()
    {
        IsDiscountVisible = !IsDiscountVisible;
        if (!IsDiscountVisible) DiscountAmount = 0;
    }

    [RelayCommand]
    private void ToggleAddCustomer()
    {
        IsAddCustomerVisible = !IsAddCustomerVisible;
        NewCustomerName = string.Empty;
    }

    [RelayCommand]
    private async Task SaveNewCustomerAsync()
    {
        if(string.IsNullOrWhiteSpace(NewCustomerName)) return;
        var cust = new Customer { Name = NewCustomerName };
        _context.Customers.Add(cust);
        await _context.SaveChangesAsync();
        
        var list = await _context.Customers.ToListAsync();
        Customers = new ObservableCollection<Customer>(list);
        SelectedCustomer = Customers.FirstOrDefault(c => c.Id == cust.Id);
        
        IsAddCustomerVisible = false;
        NewCustomerName = string.Empty;
        MessageBox.Show("تمت إضافة العميل بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Totals properties
    public int TotalItems => CartItems.Sum(i => i.Quantity);
    public decimal Subtotal => CartItems.Sum(i => i.Subtotal);
    public decimal Tax => (Subtotal - DiscountAmount) * 0.14m > 0 ? (Subtotal - DiscountAmount) * 0.14m : 0; 
    public decimal Discount => DiscountAmount;
    public decimal GrandTotal => Subtotal + Tax - DiscountAmount > 0 ? Subtotal + Tax - DiscountAmount : 0;

    private void UpdateTotals()
    {
        OnPropertyChanged(nameof(TotalItems));
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(Tax));
        OnPropertyChanged(nameof(Discount));
        OnPropertyChanged(nameof(GrandTotal));
    }
}
