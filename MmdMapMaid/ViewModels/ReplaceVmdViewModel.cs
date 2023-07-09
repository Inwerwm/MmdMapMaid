using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class ReplaceVmdViewModel : ObservableRecipient
{
    [ObservableProperty]
    VmdReplacerState _replacerState;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(WriteVmdCommand))]
    public bool _isVmdLoaded;

    [ObservableProperty]
    private InfoBarSeverity _writeVmdInfoSeverty;
    [ObservableProperty]
    private bool _openCompleteMessage;
    [ObservableProperty]
    private string _vmdWriteInfobarMessage;

    public event EventHandler<PropertyChangedEventArgs>? OnPathChanged;

    public ReplaceVmdViewModel()
    {
        _replacerState = App.GetService<VmdReplacerState>();
        _openCompleteMessage = false;
        _vmdWriteInfobarMessage = "";

        _isVmdLoaded = ReplacerState.IsVmdLoaded;

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

    private void NoticeEndWrite()
    {
        WriteVmdInfoSeverty = InfoBarSeverity.Success;
        VmdWriteInfobarMessage = "Message_VmdWriteComplete".GetLocalized();
        OpenCompleteMessage = true;
    }

    public void ReadVmd(StorageFile vmdFile)
    {
        ReplacerState.ReadVmd(vmdFile);
        IsVmdLoaded = ReplacerState.IsVmdLoaded;
        SubscribePathChanged();
    }

    [RelayCommand]
    private async Task ReadVmd()
    {
        await ReplacerState.ReadVmd();
        IsVmdLoaded = ReplacerState.IsVmdLoaded;
        SubscribePathChanged();
    }

    [RelayCommand(CanExecute = nameof(CanWriteVmdExecute))]
    private async Task WriteVmdAsync()
    {
        if (!ReplacerState.IsVmdLoaded) { return; }

        try
        {
            await ReplacerState.WriteVmd();
            NoticeEndWrite();
        }
        catch (OperationCanceledException) { }
    }

    private bool CanWriteVmdExecute() => IsVmdLoaded;

    [RelayCommand]
    private void ResetAll()
    {
        foreach (var path in ReplacerState.PathGroups.SelectMany(g => g))
        {
            path.RestorePath();
        }
    }
}
