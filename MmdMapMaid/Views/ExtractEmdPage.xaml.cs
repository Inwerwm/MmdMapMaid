using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class ExtractEmdPage : Page
{
    public ExtractEmdViewModel ViewModel
    {
        get;
    }

    public ExtractEmdPage()
    {
        ViewModel = App.GetService<ExtractEmdViewModel>();
        InitializeComponent();

        ViewModel.Extractor.SelectedEmmObjects = EmmObjectsListView.SelectedItems;
        ViewModel.Extractor.SelectedEmmEffects = EmmEffectsListView.SelectedItems;
    }
}
