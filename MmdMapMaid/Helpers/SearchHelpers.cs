using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MmdMapMaid.Controls;
using Windows.UI;

namespace MmdMapMaid.Helpers;

internal static class SearchHelpers
{

    public static void HighlightSearch(UserControl control, ListView containerListView, string query)
    {
        var theme = (control.Content as FrameworkElement)!.ActualTheme;

        foreach (var pathBox in containerListView.FindDescendants().Where(element => element is BindableRichEditBox).Cast<BindableRichEditBox>())
        {
            var range = pathBox.Document.GetRange(0, TextConstants.MaxUnitCount);

            // 背景色をリセット
            range.CharacterFormat.BackgroundColor = ((SolidColorBrush)pathBox.Background).Color;

            if (string.IsNullOrEmpty(query)) { continue; }

            while (range.FindText(query, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                range.CharacterFormat.BackgroundColor = theme switch
                {
                    ElementTheme.Light => Color.FromArgb(0x80, 0xe0, 0xe0, 0xe0),
                    ElementTheme.Dark => Color.FromArgb(0x80, 0x50, 0x50, 0x50),
                    _ => Color.FromArgb(0x80, 0x80, 0x80, 0x80),
                };
            }
        }
    }
}