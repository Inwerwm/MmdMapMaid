using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
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
        ViewModel.PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.EnableOffsetScaling):
                    ScaleOffsetArea.IsExpanded = ViewModel.EnableOffsetScaling;
                    break;
            }
        };
    }

    private void ContentArea_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void ContentArea_DropAsync(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".vmd");
        if (file is null) { return; }

        ViewModel.ReadVmd(file);
    }
}
