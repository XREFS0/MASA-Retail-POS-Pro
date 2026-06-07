using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using MASA.RetailPOS.App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MASA.RetailPOS.App.ViewModels;

public partial class InventoryViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty]
    private ObservableCollection<Product> _inventoryItems = new();

    public InventoryViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadInventoryCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadInventoryAsync()
    {
        var data = await _dbContext.Products.OrderBy(p => p.Name).ToListAsync();
        InventoryItems = new ObservableCollection<Product>(data);
    }

    [ObservableProperty]
    private Product _selectedProduct = new();

    [ObservableProperty]
    private bool _isEditing;

    [RelayCommand]
    private void AddNew()
    {
        SelectedProduct = new Product();
        IsEditing = true;
    }

    [RelayCommand]
    private void Edit(Product product)
    {
        if (product != null)
        {
            SelectedProduct = product;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedProduct.Name))
        {
            MessageBox.Show("اسم المنتج مطلوب", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedProduct.Id == 0)
        {
            _dbContext.Products.Add(SelectedProduct);
        }
        else
        {
            _dbContext.Products.Update(SelectedProduct);
        }

        await _dbContext.SaveChangesAsync();
        IsEditing = false;
        SelectedProduct = new Product();
        await LoadInventoryAsync();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        SelectedProduct = new Product();
    }

    [RelayCommand]
    private async Task DeleteAsync(Product product)
    {
        if (product == null) return;

        var result = MessageBox.Show($"هل أنت متأكد من حذف المنتج {product.Name}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            await LoadInventoryAsync();
        }
    }
}
