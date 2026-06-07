using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using MASA.RetailPOS.App.Models;
using MASA.RetailPOS.App.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MASA.RetailPOS.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BackupService _backupService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private ObservableCollection<AuditLog> _auditLogs = new();

    [ObservableProperty]
    private string _currentPassword = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private User _selectedUser = new();

    [ObservableProperty]
    private bool _isEditingUser;

    // Reset properties
    [ObservableProperty]
    private bool _clearCustomers;

    [ObservableProperty]
    private bool _clearProducts;

    [ObservableProperty]
    private bool _clearInvoices;

    [ObservableProperty]
    private bool _clearSuppliers;

    public bool CanAccessResetTab => _mainViewModel.CurrentUser?.Role == "Admin";

    public SettingsViewModel(ApplicationDbContext dbContext, BackupService backupService, MainViewModel mainViewModel)
    {
        _dbContext = dbContext;
        _backupService = backupService;
        _mainViewModel = mainViewModel;

        _ = LoadAuditLogsAsync();
        _ = LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        var usersList = await _dbContext.Users.ToListAsync();
        Users = new ObservableCollection<User>(usersList);
    }

    private async Task LoadAuditLogsAsync()
    {
        var logs = await _dbContext.AuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(50)
            .ToListAsync();
        AuditLogs = new ObservableCollection<AuditLog>(logs);
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            MessageBox.Show("يرجى إدخال كلمة المرور الحالية والجديدة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            MessageBox.Show("كلمة المرور الجديدة غير متطابقة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var user = _mainViewModel.CurrentUser;
        if (user == null) return;

        var dbUser = await _dbContext.Users.FindAsync(user.Id);
        if (dbUser == null) return;

        if (dbUser.PasswordHash != User.HashPassword(CurrentPassword))
        {
            MessageBox.Show("كلمة المرور الحالية غير صحيحة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        dbUser.PasswordHash = User.HashPassword(NewPassword);
        
        _dbContext.AuditLogs.Add(new AuditLog 
        { 
            UserId = user.Id, 
            Username = user.Username, 
            Action = "تغيير كلمة المرور", 
            Details = "قام المستخدم بتغيير كلمة المرور الخاصة به" 
        });

        await _dbContext.SaveChangesAsync();

        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmPassword = string.Empty;

        MessageBox.Show("تم تغيير كلمة المرور بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        await LoadAuditLogsAsync();
    }

    [RelayCommand]
    private void BackupDatabase()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "SQLite Database (*.db)|*.db",
            Title = "حفظ نسخة احتياطية",
            FileName = $"MASA_Backup_{System.DateTime.Now:yyyyMMdd_HHmmss}.db"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            if (_backupService.CreateManualBackup(saveFileDialog.FileName))
            {
                MessageBox.Show("تم إنشاء النسخة الاحتياطية بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                
                var user = _mainViewModel.CurrentUser;
                if (user != null)
                {
                    _dbContext.AuditLogs.Add(new AuditLog { UserId = user.Id, Username = user.Username, Action = "نسخ احتياطي", Details = "تم أخذ نسخة احتياطية يدوية" });
                    _dbContext.SaveChanges();
                    _ = LoadAuditLogsAsync();
                }
            }
            else
            {
                MessageBox.Show("حدث خطأ أثناء إنشاء النسخة الاحتياطية", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void RestoreDatabase()
    {
        MessageBoxResult result = MessageBox.Show("تحذير: استعادة النسخة الاحتياطية سيؤدي إلى استبدال كافة البيانات الحالية. هل أنت متأكد أنك تريد المتابعة؟", "تأكيد الاستعادة", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "SQLite Database (*.db)|*.db",
            Title = "اختر ملف النسخة الاحتياطية"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            if (_backupService.RestoreBackup(openFileDialog.FileName))
            {
                MessageBox.Show("تمت استعادة النسخة الاحتياطية بنجاح. سيتم الآن إغلاق التطبيق لتطبيق التغييرات. يرجى إعادة تشغيله.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("حدث خطأ أثناء استعادة النسخة الاحتياطية. تأكد من أن الملف سليم وأن التطبيق لديه صلاحيات الكتابة.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void AddNewUser()
    {
        SelectedUser = new User { Role = "Cashier", IsActive = true };
        IsEditingUser = true;
    }

    [RelayCommand]
    private void EditUser(User user)
    {
        if (user != null)
        {
            SelectedUser = user;
            IsEditingUser = true;
        }
    }

    [RelayCommand]
    private async Task SaveUserAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedUser.Username) || string.IsNullOrWhiteSpace(SelectedUser.FullName))
        {
            MessageBox.Show("اسم المستخدم والاسم الكامل مطلوبان", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedUser.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(SelectedUser.PasswordHash))
            {
                MessageBox.Show("كلمة المرور مطلوبة للمستخدم الجديد", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // For new users, the PasswordHash field from UI is raw, we must hash it.
            SelectedUser.PasswordHash = User.HashPassword(SelectedUser.PasswordHash);
            _dbContext.Users.Add(SelectedUser);
        }
        else
        {
            var dbUser = await _dbContext.Users.FindAsync(SelectedUser.Id);
            if (dbUser != null)
            {
                dbUser.FullName = SelectedUser.FullName;
                dbUser.Username = SelectedUser.Username;
                dbUser.Role = SelectedUser.Role;
                dbUser.IsActive = SelectedUser.IsActive;
                
                // Only update password if a new one was typed
                if (!string.IsNullOrWhiteSpace(SelectedUser.PasswordHash) && SelectedUser.PasswordHash != dbUser.PasswordHash)
                {
                    dbUser.PasswordHash = User.HashPassword(SelectedUser.PasswordHash);
                }
                
                _dbContext.Users.Update(dbUser);
            }
        }

        await _dbContext.SaveChangesAsync();
        IsEditingUser = false;
        SelectedUser = new User();
        await LoadUsersAsync();
    }

    [RelayCommand]
    private void CancelEditUser()
    {
        IsEditingUser = false;
        SelectedUser = new User();
    }

    [RelayCommand]
    private async Task DeleteUserAsync(User user)
    {
        if (user == null) return;
        
        var currentUser = _mainViewModel.CurrentUser;
        if (currentUser != null && currentUser.Id == user.Id)
        {
            MessageBox.Show("لا يمكنك حذف الحساب الذي تستخدمه حالياً!", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var result = MessageBox.Show($"هل أنت متأكد من حذف المستخدم {user.Username}؟", "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            await LoadUsersAsync();
        }
    }

    [RelayCommand]
    private async Task ResetDataAsync()
    {
        if (!CanAccessResetTab) return;

        if (!ClearCustomers && !ClearProducts && !ClearInvoices && !ClearSuppliers)
        {
            MessageBox.Show("يرجى تحديد عنصر واحد على الأقل ليتم مسحه.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show("تحذير خطير: الإجراء الذي ستقوم به سيمسح البيانات المحددة بشكل نهائي وسيصفر عدادات الأرقام. هل أنت متأكد بنسبة 100% أنك تريد المتابعة؟", "تأكيد مسح البيانات", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        var currentUser = _mainViewModel.CurrentUser;

        try
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            if (ClearInvoices)
            {
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM InvoiceItems;");
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Invoices;");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'InvoiceItems';");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'Invoices';");
            }

            if (ClearProducts)
            {
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Products;");
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Categories;");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'Products';");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'Categories';");
            }

            if (ClearCustomers)
            {
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Customers;");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'Customers';");
            }

            if (ClearSuppliers)
            {
                await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Suppliers;");
                await _dbContext.Database.ExecuteSqlRawAsync("UPDATE sqlite_sequence SET seq = 0 WHERE name = 'Suppliers';");
            }

            _dbContext.AuditLogs.Add(new AuditLog
            {
                UserId = currentUser!.Id,
                Username = currentUser.Username,
                Action = "مسح بيانات وتصفير",
                Details = $"تم مسح البيانات: عملاء({ClearCustomers}), منتجات({ClearProducts}), فواتير({ClearInvoices}), موردين({ClearSuppliers})"
            });

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            ClearCustomers = false;
            ClearProducts = false;
            ClearInvoices = false;
            ClearSuppliers = false;

            MessageBox.Show("تمت عملية المسح بنجاح وتم تصفير العدادات للبيانات المحددة.", "عملية ناجحة", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadAuditLogsAsync();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء المسح:\n{ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
