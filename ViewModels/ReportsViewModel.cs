using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MASA.RetailPOS.App.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty]
    private int _totalProducts;

    [ObservableProperty]
    private int _totalCustomers;

    [ObservableProperty]
    private int _totalSuppliers;

    [ObservableProperty]
    private decimal _totalSales;

    public ReportsViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadStatisticsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        TotalProducts = await _dbContext.Products.CountAsync();
        TotalCustomers = await _dbContext.Customers.CountAsync();
        TotalSuppliers = await _dbContext.Suppliers.CountAsync();
        var totalSalesDouble = await _dbContext.Invoices.SumAsync(i => (double?)i.GrandTotal) ?? 0.0;
        TotalSales = (decimal)totalSalesDouble;
    }
}
