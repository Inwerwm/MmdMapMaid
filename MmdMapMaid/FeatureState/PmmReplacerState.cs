using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Storage;

namespace MmdMapMaid.FeatureState;

public partial class PmmReplacerState : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PathGroup> _pathGroups;
    [ObservableProperty]
    private ObservableCollection<PathInformation> _modelInfo;
    [ObservableProperty]
    private ObservableCollection<PathInformation> _acsInfo;
    [ObservableProperty]
    private bool _enableEditEmm;

    private PmmPathReplacer? Replacer
    {
        get;
        set;
    }

    public bool IsPmmLoaded => Replacer != null;

    public PmmReplacerState()
    {
        _pathGroups = new();
        _modelInfo = new();
        _acsInfo = new();
        _enableEditEmm = true;
    }

    public async Task ReadPmm()
    {
        var file = await StorageHelper.PickSingleFileAsync(".pmm");
        if (file == null) { return; }

        ReadPmm(file);
    }

    public void ReadPmm(StorageFile file)
    {
        Replacer = new(file.Path);

        UpdatePathGroups();
    }

    private void UpdatePathGroups()
    {
        ModelInfo.Clear();
        foreach (var (name, path, index) in Replacer.GetModels())
        {
            ModelInfo.Add(new(index, name, path));
        }

        AcsInfo.Clear();
        foreach (var (name, path, index) in Replacer.GetAccessories())
        {
            AcsInfo.Add(new(index, name, path));
        }

        PathGroups.Clear();
        PathGroups.Add(new("DisplayName_Model".GetLocalized(), ModelInfo));
        PathGroups.Add(new("DisplayName_Accessory".GetLocalized(), AcsInfo));
    }

    public void WritePmm()
    {
        if (Replacer is null) { return; }

        foreach (var item in ModelInfo.Where(info => info.IsEdited))
        {
            Replacer.ReplaceModelPath(item.Index, item.Path, EnableEditEmm);
        }
        foreach (var item in AcsInfo.Where(info => info.IsEdited))
        {
            Replacer.ReplaceAccessoryPath(item.Index, item.Path, EnableEditEmm);
        }

        //Replacer.Remove(ModelInfo.Where(info => info.IsRemoved).Select(info => info.Index).ToArray(), AcsInfo.Where(info => info.IsRemoved).Select(info => info.Index).ToArray(), EnableEditEmm);

        Replacer.Save();

        UpdatePathGroups();
    }
}
