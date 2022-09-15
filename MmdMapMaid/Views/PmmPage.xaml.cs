using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Controls;
using MmdMapMaid.ViewModels;
using Windows.UI;

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
        var theme = (Content as FrameworkElement)!.ActualTheme;

        foreach (BindableRichEditBox pathBox in PathsListView.FindDescendants().Where(element => element is BindableRichEditBox))
        {
            var range = pathBox.Document.GetRange(0, TextConstants.MaxUnitCount);
            range.CharacterFormat.BackgroundColor = ((SolidColorBrush)pathBox.Background).Color;

            var query = ViewModel.SearchQuery;
            if (string.IsNullOrEmpty(query)) { continue; }

            while (range.FindText(query, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                range.CharacterFormat.BackgroundColor = theme switch
                {
                    ElementTheme.Light => Color.FromArgb(0xff, 0x80, 0x80, 0x80),
                    ElementTheme.Dark => Color.FromArgb(0xff, 0x80, 0x80, 0x80),
                    _ => Color.FromArgb(0xff, 0x80, 0x80, 0x80),
                };
            }
        }
    }
}
