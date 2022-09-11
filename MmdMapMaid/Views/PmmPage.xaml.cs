using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class PmmPage : Page
{
    public PmmViewModel ViewModel
    {
        get;
    }

    public PmmPage()
    {
        ViewModel = App.GetService<PmmViewModel>();
        InitializeComponent();
    }
}
