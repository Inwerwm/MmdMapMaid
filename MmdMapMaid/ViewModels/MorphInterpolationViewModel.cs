﻿using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MikuMikuMethods.Pmx;
using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Core.Models.Vmd;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Foundation;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class MorphInterpolationViewModel : ObservableRecipient
{
    private const string SettingsKeyOfMorphNames = "MorphNames";

    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    private Point _earlierPoint;

    [ObservableProperty]
    private Point _laterPoint;

    [ObservableProperty]
    private int _frameLength;

    [ObservableProperty]
    private double _accuracy;

    [ObservableProperty]
    private double _startWeight;

    [ObservableProperty]
    private double _endWeight;

    [ObservableProperty]
    private string _morphName;

    [ObservableProperty]
    private string _log;

    [ObservableProperty]
    private ObservableCollection<PathInformation> _models;

    [ObservableProperty]
    private PathInformation? _selectedModel;

    private Dictionary<PathInformation, string[]> MorphNames
    {
        get;
        set;
    }

    public Action<string>? AppendLog
    {
        get;
        set;
    }

    public MorphInterpolationViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        EarlierPoint = new(0.25, 0.25);
        LaterPoint = new(0.75, 0.75);
        StartWeight = 0;
        EndWeight = 1;
        _morphName = "";
        _log = "";

        MorphNames = _localSettingsService.ReadSetting<Dictionary<PathInformation, string[]>>(SettingsKeyOfMorphNames, Factory.CreateJsonSerializerOptions()) ?? new();
        _models = new(MorphNames.Keys);

        foreach (var model in MorphNames.Keys)
        {
            model.PropertyChanged += (s, e) => RemoveModel(s, e, model);
        }
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
            info.PropertyChanged += (s, e) => RemoveModel(s, e, info);

            Models.Add(info);
            MorphNames.Add(info, model.Morphs.Select(morph => morph.Name).ToArray());

            _localSettingsService.SaveSetting(SettingsKeyOfMorphNames, MorphNames, Factory.CreateJsonSerializerOptions());
        }
        catch
        {
            // 読み込みでエラーが起きたら何もせず終わり
        }
    }

    private void RemoveModel(object? sender, System.ComponentModel.PropertyChangedEventArgs e, PathInformation info)
    {
        if (sender is PathInformation pathInfo && pathInfo.IsRemoved)
        {
            Models.Remove(info);
            MorphNames.Remove(info);
        }

        _localSettingsService.SaveSetting(SettingsKeyOfMorphNames, MorphNames, Factory.CreateJsonSerializerOptions());
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

        var points = MorphInterpolater.CreateInterpolatedPoints(EarlierPoint.ToPoint2(), LaterPoint.ToPoint2(), FrameLength, Accuracy, 0.00001);
        MorphInterpolater.WriteVmd(savePath.Path, null, MorphName, FrameLength, points, StartWeight, EndWeight, new() { EnableBackup = false });
        AppendLog?.Invoke($"{"Log_SavePath".GetLocalized()}: {savePath.Path}");
    }
}
