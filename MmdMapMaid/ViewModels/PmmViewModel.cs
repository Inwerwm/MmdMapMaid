using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Media.Core;
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
    private string _searchQuery;
    [ObservableProperty]
    private string _replacement;

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

        _searchQuery = "";
        _replacement = "";

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
       if(e.PropertyName == nameof(PathInformation.Path)) {
            OnPathChanged?.Invoke(sender, e);
        }
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

    public void ReadPmm(StorageFile pmmFile)
    {
        ReplacerState.ReadPmm(pmmFile);
        IsPmmLoaded = true;
        SubscribePathChanged();
    }

    [RelayCommand]
    private async Task ReadPmm()
    {
        await ReplacerState.ReadPmm();
        IsPmmLoaded = true;
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

    [RelayCommand]
    private void ReplaceAll()
    {
        foreach (var pathInfo in ReplacerState.PathGroups.SelectMany(g => g))
        {
            pathInfo.Path = pathInfo.Path.Replace(SearchQuery, Replacement);
        }
    }

    private bool CanWritePmmExecute() => IsPmmLoaded;
}
