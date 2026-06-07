using System.Windows;

namespace MASA.RetailPOS.App.Views;

public partial class SplashView : Window
{
    public SplashView()
    {
        InitializeComponent();
    }

    public void UpdateProgress(string status, double progress)
    {
        Dispatcher.Invoke(() =>
        {
            StatusText.Text = status;
            ProgressBar.Value = progress;
        });
    }
}
