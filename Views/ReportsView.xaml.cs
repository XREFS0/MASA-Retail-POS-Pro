using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class ReportsView : UserControl
{
    public ReportsView(ReportsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
