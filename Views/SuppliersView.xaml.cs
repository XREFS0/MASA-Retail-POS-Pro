using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class SuppliersView : UserControl
{
    public SuppliersView(SuppliersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
