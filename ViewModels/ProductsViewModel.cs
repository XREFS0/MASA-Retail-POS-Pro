using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using MASA.RetailPOS.App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MASA.RetailPOS.App.ViewModels;

public partial class ProductsViewModel : ObservableObject
{
    private readonly ApplicationDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Product> _products = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private Product _selectedProduct = new();

    [ObservableProperty]
    private bool _isEditing;

    public ProductsViewModel(ApplicationDbContext context)
    {
        _context = context;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var categories = await _context.Categories.ToListAsync();
        Categories = new ObservableCollection<Category>(categories);

        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        Products = new ObservableCollection<Product>(products);
    }

    [RelayCommand]
    private void AddNewProduct()
    {
        SelectedProduct = new Product { SalePrice = 0, PurchasePrice = 0, StockQuantity = 0 };
        IsEditing = true;
    }

    [RelayCommand]
    private void EditProduct(Product product)
    {
        if (product != null)
        {
            SelectedProduct = product;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private async Task SaveProduct()
    {
        if (SelectedProduct == null) return;

        if (SelectedProduct.Id == 0)
        {
            _context.Products.Add(SelectedProduct);
        }
        else
        {
            _context.Products.Update(SelectedProduct);
        }

        await _context.SaveChangesAsync();
        IsEditing = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        SelectedProduct = new Product();
    }

    [RelayCommand]
    private async Task DeleteProduct(Product product)
    {
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            await LoadDataAsync();
        }
    }
}
