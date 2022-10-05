using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class ReplaceVmdPage : Page
{
    public ReplaceVmdViewModel ViewModel
    {
        get;
    }

    public ReplaceVmdPage()
    {
        ViewModel = App.GetService<ReplaceVmdViewModel>();
        InitializeComponent();
    }
}
