using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
using MmdMapMaid.ViewModels;
using Windows.Storage;

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

    private void EmmDragOver(object sender, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void TargetEmmDrop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".emm");
        if (file is null) { return; }

        ViewModel.OrderMapper.ReadEmm(file.Path);
    }

    private async void SourceModelDrop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".pmx");
        if (file is null) { return; }

        ViewModel.OrderMapper.SourcePmxPath = file.Path;
    }
    private async void DestinationModelDrop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".pmx");
        if (file is null) { return; }

        ViewModel.OrderMapper.ReadDestinationPmx(file.Path);
    }
}
