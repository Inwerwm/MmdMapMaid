using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Models;
using static ABI.System.Windows.Input.ICommand_Delegates;
using Windows.Storage;
using MmdMapMaid.Helpers;

namespace MmdMapMaid.ViewModels;

public partial class ReplaceVmdViewModel : ObservableRecipient
{
    [ObservableProperty]
    VmdReplacerState _replacerState;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(WriteVmdCommand))]
    public bool _isVmdLoaded;

    [ObservableProperty]
    private string _searchQuery;
    [ObservableProperty]
    private string _replacement;

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

        _searchQuery = "";
        _replacement = "";

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
        VmdWriteInfobarMessage = "Vmd_WriteCompleteMessage".GetLocalized();
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

    [RelayCommand]
    private void ReplaceAll()
    {
        foreach (var pathInfo in ReplacerState.PathGroups.SelectMany(g => g))
        {
            pathInfo.Path = pathInfo.Path.Replace(SearchQuery, Replacement);
        }
    }

    private bool CanWriteVmdExecute() => IsVmdLoaded;
}
