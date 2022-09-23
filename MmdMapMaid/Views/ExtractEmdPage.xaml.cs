using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
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

    private void FileDragOver(object _, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void EmmDrop(object _, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".emm");
        if (file is null) { return; }

        ViewModel.Extractor.ReadEmm(file.Path);
    }
}
