using System.Windows;
using System.Windows.Controls;

namespace MASA.RetailPOS.App.Views;

public partial class SettingsView : UserControl
{
    public SettingsView(ViewModels.SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void CurrentPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.SettingsViewModel vm)
        {
            vm.CurrentPassword = ((PasswordBox)sender).Password;
        }
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.SettingsViewModel vm)
        {
            vm.NewPassword = ((PasswordBox)sender).Password;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.SettingsViewModel vm)
        {
            vm.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}
