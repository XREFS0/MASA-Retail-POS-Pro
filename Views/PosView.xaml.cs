using System.Windows.Controls;
using MASA.RetailPOS.App.ViewModels;

namespace MASA.RetailPOS.App.Views;

public partial class PosView : UserControl
{
    public PosView(PosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
