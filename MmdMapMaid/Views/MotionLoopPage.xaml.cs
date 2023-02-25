using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class MotionLoopPage : Page
{
    public MotionLoopViewModel ViewModel
    {
        get;
    }

    public MotionLoopPage()
    {
        ViewModel = App.GetService<MotionLoopViewModel>();
        InitializeComponent();
    }
}
