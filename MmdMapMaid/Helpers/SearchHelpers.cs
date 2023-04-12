using System.Text.RegularExpressions;
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

    public static void HighlightSearch(UserControl control, ListView containerListView, string query, bool useRegex)
    {
        var theme = (control.Content as FrameworkElement)!.ActualTheme;

        foreach (var pathBox in containerListView.FindDescendants().OfType<BindableRichEditBox>())
        {
            var range = pathBox.Document.GetRange(0, TextConstants.MaxUnitCount);

            // 背景色をリセット
            range.CharacterFormat.BackgroundColor = ((SolidColorBrush)pathBox.Background).Color;

            if (string.IsNullOrEmpty(query)) { continue; }

            range.GetText(TextGetOptions.None, out var text);
            var matches = useRegex ? RegexSearch(text, query) : TextSearch(text, query);

            foreach (var matchRange in matches.Select(match => pathBox.Document.GetRange(match.Start, match.End)))
            {
                matchRange.CharacterFormat.BackgroundColor = theme switch
                {
                    ElementTheme.Light => Color.FromArgb(0x80, 0xe0, 0xe0, 0xe0),
                    ElementTheme.Dark => Color.FromArgb(0x80, 0x50, 0x50, 0x50),
                    _ => Color.FromArgb(0x80, 0x80, 0x80, 0x80),
                };
            }
        }
    }

    private static IEnumerable<(int Start, int End)> TextSearch(string input, string pattern)
    {
        var index = input.IndexOf(pattern);
        
        if(index == -1)
        {
            yield break;
        }
        yield return (index, index + pattern.Length);
    }

    private static IEnumerable<(int Start, int End)> RegexSearch(string input, string pattern)
    {
        try
        {
            var regex = new Regex(pattern);
            return regex.Matches(input).Select(match => (match.Index, match.Index + match.Length));
        }
        catch (RegexParseException)
        {
            return Enumerable.Empty<(int Start, int End)>();
        }
    }
}