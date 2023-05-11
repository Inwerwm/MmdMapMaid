using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Models;
using Windows.Foundation;

namespace MmdMapMaid.ViewModels;

public partial class MorphInterpolationViewModel : ObservableRecipient
{
    [ObservableProperty]
    private Point _earlierPoint;

    [ObservableProperty]
    private Point _laterPoint;

    [ObservableProperty]
    private int _frameLength;

    [ObservableProperty]
    private double _accuracy;

    [ObservableProperty]
    private string _morphName;

    [ObservableProperty]
    private ObservableCollection<PathInformation> _models;

    [ObservableProperty]
    private PathInformation? _selectedModel;

    private Dictionary<PathInformation, string[]> MorphNames
    {
        get;
    }

    public MorphInterpolationViewModel()
    {
        EarlierPoint = new(0.25, 0.25);
        LaterPoint = new(0.75, 0.75);

        _models = new();
        MorphNames = new();

        MakePathInfo("aaa", "bbb");
        MakePathInfo("ccc", "ddd");
    }

    private void MakePathInfo(string name, string path)
    {
        var info = new PathInformation(0, name, path);
        info.PropertyChanged += (s, e) =>
        {
            if (((PathInformation)s).IsRemoved)
            {
                Models.Remove(info);
                MorphNames.Remove(info);
            }
        };

        Models.Add(info);
        MorphNames.Add(info, new[] { name, path });
    }

    public void UpdateSuggest(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (SelectedModel is null)
        {
            return;
        }

        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        sender.ItemsSource = MorphNames[SelectedModel].Where(name => sender.Text == string.Empty || CultureInfo.CurrentCulture.CompareInfo.IndexOf(
            name,
            sender.Text,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) >= 0).ToArray();
    }

    public void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        sender.Text = args.SelectedItem.ToString();
    }
}
