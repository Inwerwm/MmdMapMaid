using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class EmmPage : Page
{
    public EmmViewModel ViewModel
    {
        get;
    }

    public EmmPage()
    {
        ViewModel = App.GetService<EmmViewModel>();
        InitializeComponent();

        ViewModel.OrderMapper.SelectedEmmModels = EmmModelsListView.SelectedItems;
    }
}
