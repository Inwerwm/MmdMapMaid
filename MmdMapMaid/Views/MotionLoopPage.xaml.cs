using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
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
        ViewModel.AppendLog = (log) =>
        {
            TextBoxLog.Text = $"""
            {DateTime.Now}
            {log}

            {TextBoxLog.Text}
            """;
        };
    }

    private void ContentArea_DragOver(object _, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void ContentArea_Drop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".vmd");
        if (file is null) { return; }

        ViewModel.ReadVmd(file);
    }
}
