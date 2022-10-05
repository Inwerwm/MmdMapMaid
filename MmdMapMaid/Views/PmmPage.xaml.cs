using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
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

        ViewModel.OnPathChanged += (_, _) =>
        {
            Search();
        };
    }

    private void SearchQueryChanged(object sender, TextChangedEventArgs e)
    {
        Search();
    }

    private void Search()
    {
        SearchHelpers.HighlightSearch(this, PathsListView, ViewModel.SearchQuery);
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
}
