using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class PmmViewModel : ObservableRecipient
{
    [ObservableProperty]
    PmmReplacerState _replacerState;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(WritePmmCommand))]
    public bool _isPmmLoaded;

    [ObservableProperty]
    private InfoBarSeverity _writePmmInfoSeverty;
    [ObservableProperty]
    private bool _openCompleteMessage;
    [ObservableProperty]
    private string _pmmWriteInfobarMessage;

    public event EventHandler<PropertyChangedEventArgs>? OnPathChanged;

    public PmmViewModel()
    {
        _replacerState = App.GetService<PmmReplacerState>();
        _openCompleteMessage = false;
        _pmmWriteInfobarMessage = "";

        _isPmmLoaded = ReplacerState.IsPmmLoaded;

        SubscribePathChanged();
    }

    private void SubscribePathChanged()
    {
        foreach (var pathInfo in ReplacerState.PathGroups.SelectMany(g => g))
        {
            pathInfo.PropertyChanged += PathInfoChanged;
        }
    }

    private void PathInfoChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PathInformation.Path))
        {
            OnPathChanged?.Invoke(sender, e);
        }
    }

    private void NoticeStartWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Informational;
        PmmWriteInfobarMessage = "Message_Writing".GetLocalized();
        OpenCompleteMessage = true;
    }

    private void NoticeEndWrite()
    {
        WritePmmInfoSeverty = InfoBarSeverity.Success;
        PmmWriteInfobarMessage = "Message_PmmWriteComplete".GetLocalized();
        OpenCompleteMessage = true;
    }

    public void ReadPmm(StorageFile pmmFile)
    {
        ReplacerState.ReadPmm(pmmFile);
        IsPmmLoaded = ReplacerState.IsPmmLoaded;
        SubscribePathChanged();
    }

    [RelayCommand]
    private async Task ReadPmm()
    {
        await ReplacerState.ReadPmm();
        IsPmmLoaded = ReplacerState.IsPmmLoaded;
        SubscribePathChanged();
    }

    [RelayCommand(CanExecute = nameof(CanWritePmmExecute))]
    private void WritePmm()
    {
        if (!ReplacerState.IsPmmLoaded) { return; }

        NoticeStartWrite();

        ReplacerState.WritePmm();

        NoticeEndWrite();
    }

    private bool CanWritePmmExecute() => IsPmmLoaded;
}
