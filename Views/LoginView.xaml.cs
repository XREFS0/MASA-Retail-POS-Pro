using System.Windows.Controls;
using System.Windows;

namespace MASA.RetailPOS.App.Views;

public partial class LoginView : UserControl
{
    public LoginView(ViewModels.LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.LoginViewModel vm)
        {
            vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
