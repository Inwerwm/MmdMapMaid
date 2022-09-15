using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;

namespace MmdMapMaid.FeatureState;

[INotifyPropertyChanged]
public partial class PmmReplacerState
{
    [ObservableProperty]
    private ObservableCollection<PathGroup> _pathGroups;
    [ObservableProperty]
    private ObservableCollection<PathInformation> _modelInfo;

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
    }

    public async Task ReadPmm()
    {
        var file = await StorageHelper.PickSingleFileAsync(".pmm");
        if (file == null) { return; }

        Replacer = new(file.Path);

        ModelInfo.Clear();
        foreach (var (name, path, index) in Replacer.GetModels())
        {
            ModelInfo.Add(new(index, name, path));
        }

        PathGroups.Clear();
        PathGroups.Add(new("DisplayName_Model".GetLocalized(), ModelInfo));
    }

    public void WritePmm()
    {
        if (Replacer is null) { return; }

        foreach (var item in ModelInfo.Where(info => info.IsEdited))
        {
            Replacer.ReplaceModelPath(item.Index, item.Path);
        }

        Replacer.Save();
    }
}
