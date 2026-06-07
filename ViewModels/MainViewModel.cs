using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MASA.RetailPOS.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _currentTime = DateTime.Now.ToString("hh:mm:ss tt");

    [ObservableProperty]
    private string _currentDate = DateTime.Now.ToString("yyyy-MM-dd");

    [ObservableProperty]
    private MASA.RetailPOS.App.Models.User? _currentUser;

    public bool IsLoggedIn => CurrentUser != null;

    // Permissions
    public bool CanAccessSettings => CurrentUser?.Role == "Admin";
    public bool CanAccessReports => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager";
    public bool CanAccessInventory => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager" || CurrentUser?.Role == "Warehouse";
    public bool CanAccessSuppliers => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager";
    public bool CanAccessProducts => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager" || CurrentUser?.Role == "Cashier" || CurrentUser?.Role == "Warehouse";
    public bool CanAccessPos => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager" || CurrentUser?.Role == "Cashier";
    public bool CanAccessDashboard => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager";

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        
        using (var dbContext = new MASA.RetailPOS.App.Data.ApplicationDbContext())
        {
            dbContext.Database.EnsureCreated();
        }

        // Setup initial view to Login
        CurrentView = _serviceProvider.GetRequiredService<Views.LoginView>();
    }

    public void SetCurrentUser(MASA.RetailPOS.App.Models.User user)
    {
        CurrentUser = user;
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(CanAccessSettings));
        OnPropertyChanged(nameof(CanAccessReports));
        OnPropertyChanged(nameof(CanAccessInventory));
        OnPropertyChanged(nameof(CanAccessSuppliers));
        OnPropertyChanged(nameof(CanAccessProducts));
        OnPropertyChanged(nameof(CanAccessPos));
        OnPropertyChanged(nameof(CanAccessDashboard));

        // Navigate to default view based on role
        if (CanAccessDashboard)
            NavigateToDashboard();
        else if (CanAccessPos)
            NavigateToPos();
        else if (CanAccessInventory)
            NavigateToInventory();
    }

    [RelayCommand]
    private void Logout()
    {
        if (CurrentUser != null)
        {
            using (var dbContext = new MASA.RetailPOS.App.Data.ApplicationDbContext())
            {
                dbContext.AuditLogs.Add(new MASA.RetailPOS.App.Models.AuditLog 
                { 
                    UserId = CurrentUser.Id, 
                    Username = CurrentUser.Username, 
                    Action = "تسجيل خروج", 
                    Details = $"تم تسجيل خروج المستخدم" 
                });
                dbContext.SaveChanges();
            }
        }
        
        CurrentUser = null;
        OnPropertyChanged(nameof(IsLoggedIn));
        CurrentView = _serviceProvider.GetRequiredService<Views.LoginView>();
    }

    [RelayCommand]
    private void NavigateToPos()
    {
        if (!CanAccessPos) return;
        try
        {
            CurrentView = _serviceProvider.GetRequiredService<Views.PosView>();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.ToString(), "Error Loading PosView");
        }
    }

    [RelayCommand]
    private void NavigateToProducts()
    {
        if (!CanAccessProducts) return;
        try
        {
            CurrentView = _serviceProvider.GetRequiredService<Views.ProductsView>();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.ToString(), "Error Loading ProductsView");
        }
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        if (!CanAccessDashboard) return;
        CurrentView = _serviceProvider.GetRequiredService<Views.DashboardView>();
    }

    [RelayCommand]
    private void NavigateToInventory()
    {
        if (!CanAccessInventory) return;
        CurrentView = _serviceProvider.GetRequiredService<Views.InventoryView>();
    }

    [RelayCommand]
    private void NavigateToCustomers()
    {
        CurrentView = _serviceProvider.GetRequiredService<Views.CustomersView>();
    }

    [RelayCommand]
    private void NavigateToSuppliers()
    {
        if (!CanAccessSuppliers) return;
        CurrentView = _serviceProvider.GetRequiredService<Views.SuppliersView>();
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        if (!CanAccessReports) return;
        CurrentView = _serviceProvider.GetRequiredService<Views.ReportsView>();
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        if (!CanAccessSettings) return;
        CurrentView = _serviceProvider.GetRequiredService<Views.SettingsView>();
    }
}
