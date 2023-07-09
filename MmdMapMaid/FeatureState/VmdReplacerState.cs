using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Core.Models.Vmd;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Storage;

namespace MmdMapMaid.FeatureState;

public partial class VmdReplacerState : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PathGroup> _pathGroups;
    [ObservableProperty]
    private ObservableCollection<PathInformation> _motionInfo;
    [ObservableProperty]
    private ObservableCollection<PathInformation> _morphInfo;

    private VmdReplacer? Replacer
    {
        get;
        set;
    }

    public bool IsVmdLoaded => Replacer != null;

    public VmdReplacerState()
    {
        _pathGroups = new();
        _motionInfo = new();
        _morphInfo = new();
    }

    public async Task ReadVmd()
    {
        var file = await StorageHelper.PickSingleFileAsync(".vmd");
        if (file == null) { return; }

        ReadVmd(file);
    }

    public void ReadVmd(StorageFile file)
    {
        Replacer = new(file.Path);
        UpdateGroups();
    }

    private void UpdateGroups()
    {
        if (Replacer is null) { return; }

        MotionInfo.Clear();
        foreach (var (name, i) in Replacer.GetMotions().Select((name, i) => (name, i)))
        {
            MotionInfo.Add(new(i, name, name));
        }

        MorphInfo.Clear();
        foreach (var (name, i) in Replacer.GetMorphs().Select((name, i) => (name, i)))
        {
            MorphInfo.Add(new(i, name, name));
        }

        PathGroups.Clear();
        PathGroups.Add(new("DisplayName_Motion".GetLocalized(), MotionInfo));
        PathGroups.Add(new("DisplayName_Morph".GetLocalized(), MorphInfo));
    }

    public async Task WriteVmd()
    {
        if (Replacer is null) { return; }

        var savePath = await StorageHelper.PickSaveFileAsync(new KeyValuePair<string, IList<string>>("VMD ファイル", new[] { ".vmd" }));
        if (savePath is null) { throw new OperationCanceledException(); }

        foreach (var item in MotionInfo.Where(info => info.IsEdited))
        {
            if (item.IsRemoved)
            {
                Replacer.RemoveMotion(item.Name);
            }
            else
            {
                Replacer.ReplaceMotion(item.Name, item.Path);
            }
        }
        foreach (var item in MorphInfo.Where(info => info.IsEdited))
        {
            if (item.IsRemoved)
            {
                Replacer.RemoveMoprh(item.Name);
            }
            else
            {
                Replacer.ReplaceMorph(item.Name, item.Path);
            }
        }

        Replacer.Save(savePath.Path);

        UpdateGroups();
    }
}
