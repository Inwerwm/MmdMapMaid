using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;
using Windows.Media.Core;

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

    public PmmViewModel()
    {
        _replacerState = App.GetService<PmmReplacerState>();
        _openCompleteMessage = false;
        _pmmWriteInfobarMessage = "";

        _isPmmLoaded = ReplacerState.IsPmmLoaded;
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

    [RelayCommand]
    private async Task ReadPmm()
    {
        await ReplacerState.ReadPmm();
        IsPmmLoaded = true;
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
