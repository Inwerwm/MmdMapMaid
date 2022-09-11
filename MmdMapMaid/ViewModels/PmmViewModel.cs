﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Observables;

namespace MmdMapMaid.ViewModels;

public partial class PmmViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<PmmModelInformation> _modelInfo;

    [ObservableProperty]
    private InfoBarSeverity _writePmmInfoSeverty;
    [ObservableProperty]
    private bool _openCompleteMessage;
    [ObservableProperty]
    private string _writingMessage;

    private ModelReplacer? Replacer
    {
        get;
        set;
    }

    public PmmViewModel()
    {
        _modelInfo = new();
        _openCompleteMessage = false;
    }

    private void NoticeStartWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Informational;
        WritingMessage = "Writing...";
        OpenCompleteMessage = true;
    }

    private void NoticeEndWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Success;
        WritingMessage = "Pmm file has been written.";
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

        foreach (var item in ModelInfo)
        {
            Replacer.Replace(item.Index, item.Path);
        }

        Replacer.Save();

        NoticeEndWrite();
    }
}
