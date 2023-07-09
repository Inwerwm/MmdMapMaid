using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Controls;

namespace MmdMapMaid.ViewModels;

public partial class ReplacementBoxViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _searchQuery;
    [ObservableProperty]
    private string _replacement;
    [ObservableProperty]
    private bool _useRegex;

    public BindableRichEditBox[]? RichEditBoxes
    {
        get;
        set;
    }

    public ReplacementBoxViewModel()
    {
        _searchQuery = string.Empty;
        _replacement = string.Empty;
        _useRegex = true;
    }

    [RelayCommand]
    private void ReplaceAll()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) { return; }
        if (RichEditBoxes is null) { return; }

        foreach (var box in RichEditBoxes)
        {
            box.Text = UseRegex ? Regex.Replace(box.Text, SearchQuery, Replacement) : box.Text.Replace(SearchQuery, Replacement);
        }
    }
}
