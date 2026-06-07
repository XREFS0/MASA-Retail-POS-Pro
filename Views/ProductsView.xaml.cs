using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class ProductsView : UserControl
{
    public ProductsView(ProductsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
