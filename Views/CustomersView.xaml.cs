using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class CustomersView : UserControl
{
    public CustomersView(CustomersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
