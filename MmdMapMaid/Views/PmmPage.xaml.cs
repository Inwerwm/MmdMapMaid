using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Controls;
using MmdMapMaid.Helpers;
using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class PmmPage : Page
{
    public PmmViewModel ViewModel
    {
        get;
    }

    private BindableRichEditBox[] PathBoxes
    {
        get;
        set;
    }

    public PmmPage()
    {
        ViewModel = App.GetService<PmmViewModel>();
        InitializeComponent();

        ViewModel.OnPathChanged += (_, _) =>
        {
            Search();
        };
        PathsListView.LayoutUpdated += (_, _) => PathBoxes = PathsListView.FindDescendants().OfType<BindableRichEditBox>().ToArray();
    }

    private void SearchQueryChanged(object sender, TextChangedEventArgs e)
    {
        Search();
    }

    private void Search()
    {
        SearchHelpers.HighlightSearch(this, PathBoxes, ViewModel.SearchQuery, UseRegex.IsOn);
    }

    private void ContentArea_DragOver(object _, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void ContentArea_Drop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".pmm");
        if (file is null) { return; }

        ViewModel.ReadPmm(file);
    }

    private void UseRegex_Toggled(object sender, RoutedEventArgs e)
    {
        Search();
    }
}
