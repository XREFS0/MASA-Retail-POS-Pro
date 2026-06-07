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

public partial class CustomersViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    [ObservableProperty]
    private Customer _selectedCustomer = new();

    [ObservableProperty]
    private bool _isEditing;

    public CustomersViewModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadCustomersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCustomersAsync()
    {
        var data = await _dbContext.Customers.ToListAsync();
        Customers = new ObservableCollection<Customer>(data);
    }

    [RelayCommand]
    private void AddNew()
    {
        SelectedCustomer = new Customer();
        IsEditing = true;
    }

    [RelayCommand]
    private void Edit(Customer customer)
    {
        if (customer != null)
        {
            SelectedCustomer = customer;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedCustomer.Name))
        {
            MessageBox.Show("اسم العميل مطلوب", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedCustomer.Id == 0)
        {
            _dbContext.Customers.Add(SelectedCustomer);
        }
        else
        {
            _dbContext.Customers.Update(SelectedCustomer);
        }

        await _dbContext.SaveChangesAsync();
        IsEditing = false;
        SelectedCustomer = new Customer();
        await LoadCustomersAsync();
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        SelectedCustomer = new Customer();
    }

    [RelayCommand]
    private async Task DeleteAsync(Customer customer)
    {
        if (customer == null) return;

        var result = MessageBox.Show($"هل أنت متأكد من حذف العميل {customer.Name}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            await LoadCustomersAsync();
        }
    }
}
