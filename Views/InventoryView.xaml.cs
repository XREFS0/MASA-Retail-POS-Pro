using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class InventoryView : UserControl
{
    public InventoryView(InventoryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
