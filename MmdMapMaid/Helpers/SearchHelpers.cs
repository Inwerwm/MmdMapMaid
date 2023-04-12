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

        foreach (var pathBox in containerListView.FindDescendants().Where(element => element is BindableRichEditBox).Cast<BindableRichEditBox>())
        {
            var range = pathBox.Document.GetRange(0, TextConstants.MaxUnitCount);

            // 背景色をリセット
            range.CharacterFormat.BackgroundColor = ((SolidColorBrush)pathBox.Background).Color;

            if (string.IsNullOrEmpty(query)) { continue; }

            if (useRegex)
            {
                RegexSearch(query, theme, range, pathBox);
            }
            else
            {
                TextSearch(query, theme, range);
            }
        }
    }

    private static void TextSearch(string query, ElementTheme theme, ITextRange range)
    {
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

    private static void RegexSearch(string query, ElementTheme theme, ITextRange range, RichEditBox pathBox)
    {
        // 正規表現オブジェクトを作成
        var regex = new Regex(query);

        // テキストボックスのテキストを取得
        range.GetText(TextGetOptions.None, out var text);

        // 正規表現で一致するテキストを探す
        foreach (var matchRange in regex.Matches(text).Select(match => pathBox.Document.GetRange(match.Index, match.Index + match.Length)))
        {
            // 背景色を変更
            matchRange.CharacterFormat.BackgroundColor = theme switch
            {
                ElementTheme.Light => Color.FromArgb(0x80, 0xe0, 0xe0, 0xe0),
                ElementTheme.Dark => Color.FromArgb(0x80, 0x50, 0x50, 0x50),
                _ => Color.FromArgb(0x80, 0x80, 0x80, 0x80),
            };
        }
    }
}