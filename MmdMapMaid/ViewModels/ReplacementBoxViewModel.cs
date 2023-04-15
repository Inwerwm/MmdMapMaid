using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Controls;
using MmdMapMaid.Models;

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
    }

    [RelayCommand]
    private void ReplaceAll()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) { return; }
        if(RichEditBoxes is null) { return; }

        foreach (var box in RichEditBoxes)
        {
            box.Text = UseRegex ? Regex.Replace(box.Text, SearchQuery, Replacement) : box.Text.Replace(SearchQuery, Replacement);
        }
    }
}
