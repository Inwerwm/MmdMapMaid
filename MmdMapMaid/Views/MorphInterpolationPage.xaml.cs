using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Helpers;
using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class MorphInterpolationPage : Page
{
    public MorphInterpolationViewModel ViewModel
    {
        get;
    }

    public MorphInterpolationPage()
    {
        ViewModel = App.GetService<MorphInterpolationViewModel>();
        InitializeComponent();
    }

    private void MorphNameSuggestBox_GotFocus(object sender, RoutedEventArgs e)
    {
        ViewModel.UpdateSuggest(MorphNameSuggestBox, new()
        {
            Reason = AutoSuggestionBoxTextChangeReason.UserInput
        });
        MorphNameSuggestBox.IsSuggestionListOpen = true;
    }

    private void Grid_DragOver(object sender, DragEventArgs e)
    {
        StorageHelper.SetAcceptedOperation(e);
    }

    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        var file = await StorageHelper.ReadDropedFile(e, ".pmx");
        if (file is null) { return; }

        await ViewModel.ReadPmxAsync(file);
    }
}
