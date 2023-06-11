using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class VmdRangeEditPage : Page
{
    public VmdRangeEditViewModel ViewModel
    {
        get;
    }

    public VmdRangeEditPage()
    {
        ViewModel = App.GetService<VmdRangeEditViewModel>();
        InitializeComponent();
    }
}
