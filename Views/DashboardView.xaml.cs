using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class DashboardView : UserControl
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
