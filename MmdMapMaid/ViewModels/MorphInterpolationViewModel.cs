using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MikuMikuMethods.Pmx;
using MmdMapMaid.Core.Models.Vmd;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Foundation;
using Windows.Storage;

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
        MorphName = "";

        _models = new();
        MorphNames = new();
    }

    public void UpdateSuggest(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (SelectedModel is null)
        {
            sender.ItemsSource = null;
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

    public void ReadPmx(StorageFile file)
    {
        try
        {
            if (!File.Exists(file.Path)) { return; }

            var model = new PmxModel(file.Path);

            var info = new PathInformation(0, model.ModelInfo.Name, file.Path);
            info.PropertyChanged += (s, e) =>
            {
                if (((PathInformation)s).IsRemoved)
                {
                    Models.Remove(info);
                    MorphNames.Remove(info);
                }
            };

            Models.Add(info);
            MorphNames.Add(info, model.Morphs.Select(morph => morph.Name).ToArray());
        }
        catch
        {
            // 読み込みでエラーが起きたら何もせず終わり
        }
    }

    [RelayCommand]
    private async Task ReadPmxAsync()
    {
        var pmxFile = await StorageHelper.PickSingleFileAsync(".pmx");
        if (pmxFile is null) { return; }
        ReadPmx(pmxFile);
    }

    [RelayCommand]
    private async Task WriteVmdAsync()
    {
        var savePath = await StorageHelper.PickSaveFileAsync($"ip_{MorphName}.vmd", new KeyValuePair<string, IList<string>>("VMD ファイル", new[] { ".vmd" }));
        if (savePath is null) { return; }

        var points = MorphInterpolater.CreateInterpolatedPoints(EarlierPoint.ToPoint2(), LaterPoint.ToPoint2(), FrameLength, Accuracy);
        MorphInterpolater.WriteVmd(savePath.Path, null, MorphName, FrameLength, points.Prepend(MorphInterpolater.StartPoint).Append(MorphInterpolater.EndPoint), new() { EnableBackup = false });
    }
}
