using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MASA.RetailPOS.App.Data;
using MASA.RetailPOS.App.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MASA.RetailPOS.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApplicationDbContext _dbContext;
    private readonly System.IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(ApplicationDbContext dbContext, System.IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "يرجى إدخال اسم المستخدم وكلمة المرور";
            return;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == Username);

        if (user == null)
        {
            ErrorMessage = "اسم المستخدم غير موجود";
            return;
        }

        var hashedPassword = User.HashPassword(Password);
        
        if (user.PasswordHash != hashedPassword)
        {
            ErrorMessage = "كلمة المرور غير صحيحة";
            return;
        }

        user.LastLogin = System.DateTime.Now;
        
        _dbContext.AuditLogs.Add(new AuditLog 
        { 
            UserId = user.Id, 
            Username = user.Username, 
            Action = "تسجيل دخول", 
            Details = $"تم تسجيل دخول المستخدم ({user.Role})" 
        });
        
        await _dbContext.SaveChangesAsync();

        // Set user and navigate to initial view based on role
        var mainViewModel = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainViewModel>(_serviceProvider);
        mainViewModel.SetCurrentUser(user);
    }
}
