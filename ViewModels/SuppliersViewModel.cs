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

public partial class SuppliersViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty]
    private ObservableCollection<Supplier> _suppliers = new();

    [ObservableProperty]
    private Supplier _selectedSupplier = new();

    [ObservableProperty]
    private bool _isEditing;

    public SuppliersViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadSuppliersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSuppliersAsync()
    {
        var data = await _dbContext.Suppliers.ToListAsync();
        Suppliers = new ObservableCollection<Supplier>(data);
    }

    [RelayCommand]
    private void AddNew()
    {
        SelectedSupplier = new Supplier();
        IsEditing = true;
    }

    [RelayCommand]
    private void Edit(Supplier supplier)
    {
        if (supplier != null)
        {
            SelectedSupplier = supplier;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedSupplier.Name))
        {
            MessageBox.Show("اسم المورد مطلوب", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedSupplier.Id == 0)
        {
            _dbContext.Suppliers.Add(SelectedSupplier);
        }
        else
        {
            _dbContext.Suppliers.Update(SelectedSupplier);
        }

        await _dbContext.SaveChangesAsync();
        IsEditing = false;
        SelectedSupplier = new Supplier();
        await LoadSuppliersAsync();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        SelectedSupplier = new Supplier();
    }

    [RelayCommand]
    private async Task DeleteAsync(Supplier supplier)
    {
        if (supplier == null) return;

        var result = MessageBox.Show($"هل أنت متأكد من حذف المورد {supplier.Name}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _dbContext.Suppliers.Remove(supplier);
            await _dbContext.SaveChangesAsync();
            await LoadSuppliersAsync();
        }
    }
}
