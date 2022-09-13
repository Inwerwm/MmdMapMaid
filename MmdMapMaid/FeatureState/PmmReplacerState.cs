using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Observables;

namespace MmdMapMaid.FeatureState;
public partial class PmmReplacerState : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<PmmModelInformation> _modelInfo;

    [ObservableProperty]
    private InfoBarSeverity _writePmmInfoSeverty;
    [ObservableProperty]
    private bool _openCompleteMessage;
    [ObservableProperty]
    private string _pmmWriteInfobarMessage;

    private ModelReplacer? Replacer
    {
        get;
        set;
    }

    public PmmReplacerState()
    {
        _modelInfo = new();
        _openCompleteMessage = false;
        _pmmWriteInfobarMessage = "";
    }

    private void NoticeStartWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Informational;
        PmmWriteInfobarMessage = "PmmWriteInfobarMessage_Writing".GetLocalized();
        OpenCompleteMessage = true;
    }

    private void NoticeEndWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Success;
        PmmWriteInfobarMessage = "PmmWriteInfobarMessage_Finished".GetLocalized();
        OpenCompleteMessage = true;
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

    [ICommand]
    private void WritePmm()
    {
        if (Replacer is null) { return; }

        NoticeStartWrite();

        foreach (var item in ModelInfo.Where(info => info.IsEdited))
        {
            Replacer.Replace(item.Index, item.Path);
        }

        Replacer.Save();

        NoticeEndWrite();
    }
}
