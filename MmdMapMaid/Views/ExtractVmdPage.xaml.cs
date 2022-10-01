using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class ExtractVmdPage : Page
{
    public ExtractVmdViewModel ViewModel
    {
        get;
    }

    public ExtractVmdPage()
    {
        ViewModel = App.GetService<ExtractVmdViewModel>();
        InitializeComponent();

        ViewModel.Extractor.DispatcherQueue = DispatcherQueue;
        ViewModel.Extractor.SelectedPmmModels = PmmModelsListView.SelectedItems;
    }

    private void FileDragOver(object _, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void PmmDrop(object _, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".pmm");
        if (file is null) { return; }

        ViewModel.Extractor.ReadPmm(file.Path);
    }
}
