using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MASA.RetailPOS.App.ViewModels;

public class TopProductItem
{
    public string Name { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalSales { get; set; }
}

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private decimal _totalSales;

    [ObservableProperty]
    private int _invoiceCount;

    [ObservableProperty]
    private decimal _totalProfits;

    [ObservableProperty]
    private int _lowStockItems;

    [ObservableProperty]
    private ISeries[] _salesSeries = System.Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] _xAxes = System.Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _yAxes = new Axis[]
    {
        new Axis
        {
            LabelsPaint = new SolidColorPaint(SKColors.White),
            SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255, 30)) { StrokeThickness = 1 }
        }
    };

    [ObservableProperty]
    private ObservableCollection<TopProductItem> _topProducts = new();

    private readonly MASA.RetailPOS.App.Data.ApplicationDbContext _dbContext;

    public DashboardViewModel(MASA.RetailPOS.App.Data.ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _ = LoadDashboardDataAsync();
    }

    private async System.Threading.Tasks.Task LoadDashboardDataAsync()
    {
        try
        {
            TotalSales = (decimal)await _dbContext.Invoices.SumAsync(i => (double)i.GrandTotal);
            InvoiceCount = await _dbContext.Invoices.CountAsync();
            TotalProfits = TotalSales * 0.25m; // Estimated profit margin
            LowStockItems = await _dbContext.Products.CountAsync(p => p.StockQuantity <= 10);

            // Chart Data: Last 7 Days
            var last7Days = System.DateTime.Now.Date.AddDays(-6);
            var salesData = await _dbContext.Invoices
                .Where(i => i.InvoiceDate >= last7Days && i.Status == "Completed")
                .GroupBy(i => i.InvoiceDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(i => (double?)i.GrandTotal) ?? 0.0 })
                .ToListAsync();

            var values = new double[7];
            var labels = new string[7];
            for (int i = 0; i < 7; i++)
            {
                var d = last7Days.AddDays(i);
                labels[i] = d.ToString("MM/dd");
                var s = salesData.FirstOrDefault(x => x.Date == d);
                values[i] = s?.Total ?? 0.0;
            }

            SalesSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = values,
                    Name = "المبيعات",
                    Stroke = new SolidColorPaint(SKColors.BlueViolet) { StrokeThickness = 3 },
                    Fill = new SolidColorPaint(SKColors.BlueViolet.WithAlpha(50)),
                    GeometrySize = 10,
                    GeometryStroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 },
                    LineSmoothness = 0.5
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TextSize = 14,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255, 30)) { StrokeThickness = 1 }
                }
            };

            // Top Products
            var allItems = await _dbContext.InvoiceItems.Include(i => i.Product).ToListAsync();
            var top = allItems
                .GroupBy(i => new { i.ProductId, Name = i.Product != null ? i.Product.Name : "غير معروف" })
                .Select(g => new TopProductItem
                {
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(i => i.Quantity),
                    TotalSales = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToList();

            TopProducts = new ObservableCollection<TopProductItem>(top);
        }
        catch (System.Exception ex)
        {
            // Ignore or log error
            System.Console.WriteLine(ex.Message);
        }
    }
}
