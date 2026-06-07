using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using MASA.RetailPOS.App.ViewModels;
using MASA.RetailPOS.App.Data;

namespace MASA.RetailPOS.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    public App()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Backup Service
        var backupService = new MASA.RetailPOS.App.Services.BackupService();
        backupService.RunDailyBackup();
        services.AddSingleton(backupService);

        // DbContext
        services.AddDbContext<ApplicationDbContext>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<PosViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<CustomersViewModel>();
        services.AddTransient<SuppliersViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<Views.LoginView>();
        services.AddTransient<Views.PosView>();
        services.AddTransient<Views.ProductsView>();
        services.AddTransient<Views.DashboardView>();
        services.AddTransient<Views.InventoryView>();
        services.AddTransient<Views.CustomersView>();
        services.AddTransient<Views.SuppliersView>();
        services.AddTransient<Views.ReportsView>();
        services.AddTransient<Views.SettingsView>();

        return services.BuildServiceProvider();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var splash = new Views.SplashView();
            splash.Show();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            await System.Threading.Tasks.Task.Run(async () =>
            {
                splash.UpdateProgress("تهيئة بيئة العمل...", 10);
                await System.Threading.Tasks.Task.Delay(500);

                using (var scope = Services.CreateScope())
                {
                    splash.UpdateProgress("جاري الاتصال بقاعدة البيانات...", 30);
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    splash.UpdateProgress("جاري التحقق من تحديثات الجداول...", 50);
                    dbContext.Database.EnsureCreated();
                    
                    try
                    {
                        Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRaw(dbContext.Database, "ALTER TABLE Users ADD COLUMN IsActive INTEGER NOT NULL DEFAULT 1;");
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllText("migration_error.txt", ex.ToString());
                    }

                    splash.UpdateProgress("جاري تجهيز واجهة المستخدم...", 80);
                    await System.Threading.Tasks.Task.Delay(500); // Give user a moment to see the loading bar
                }
            });

            splash.UpdateProgress("اكتمل التحميل!", 100);
            await System.Threading.Tasks.Task.Delay(200);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
            
            splash.Close();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FATAL ERROR: {ex.Message}\n{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"INNER: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}");
            }
            Application.Current.Shutdown();
        }
    }
}
