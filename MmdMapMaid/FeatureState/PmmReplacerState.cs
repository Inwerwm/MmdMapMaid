using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Observables;

namespace MmdMapMaid.FeatureState;
public partial class PmmReplacerState : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<PmmModelInformation> _modelInfo;

    private ModelReplacer? Replacer
    {
        get;
        set;
    }

    public bool IsPmmLoaded => Replacer != null;

    public PmmReplacerState()
    {
        _modelInfo = new();
    }

    [ICommand]
    private async Task ReadPmm()
    {
        var file = await StorageHelper.PickSingleFileAsync(".pmm");
        if (file == null) { return; }

        Replacer = new(file.Path);

        ModelInfo.Clear();
        foreach (var (name, path, index) in Replacer.GetModelList())
        {
            ModelInfo.Add(new(index, name, path));
        }
    }

    public void WritePmm()
    {
        if (Replacer is null) { return; }

        foreach (var item in ModelInfo.Where(info => info.IsEdited))
        {
            Replacer.Replace(item.Index, item.Path);
        }

        Replacer.Save();
    }
}
