using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
}
