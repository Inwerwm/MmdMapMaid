using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using MikuMikuMethods.Converter;
using MmdMapMaid.Core.Models.Pmm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;

namespace MmdMapMaid.FeatureState;

[INotifyPropertyChanged]
internal partial class VmdExtractorState
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private string? _pmmPath;

    [ObservableProperty]
    private int _lastFrame;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _doesExtractCamera;
    [ObservableProperty]
    private bool _doesExtractLight;

    [ObservableProperty]
    private ObservableCollection<PathInformation> _pmmModels;
    internal IList<object>? SelectedPmmModels
    {
        get;
        set;
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _isModelSelected;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _doesExtractMotion;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _doesExtractMorph;

    [ObservableProperty]
    private uint _startFrame;
    [ObservableProperty]
    private int _endFrame;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private string _saveDirectory;
    private bool _isSaveDirectoryExplicitlySelected;

    public event EventHandler? OnCompleted;

    private CancellationTokenSource? LastFrameCancellation
    {
        get;
        set;
    }

    private VmdExtractor? Extractor
    {
        get;
        set;
    }

    public DispatcherQueue? DispatcherQueue
    {
        get; set;
    }

    public VmdExtractorState()
    {
        _pmmModels = new();
        _saveDirectory = "";
        _isSaveDirectoryExplicitlySelected = false;

        _startFrame = 0;
        _endFrame = -1;
        _lastFrame = int.MaxValue;
    }

    public void UpdateSelectedModels(object _, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        IsModelSelected = SelectedPmmModels?.Any() ?? false;
    }

    public void ReadPmm(string pmmPath)
    {
        PmmPath = pmmPath;
        Extractor = new(PmmPath);
        LastFrame = int.MaxValue;
        ReadLastFrame();

        PmmModels.Clear();
        foreach (var (name, path, i) in Extractor.GetModels())
        {
            PmmModels.Add(new(i, name, path));
        }

        if (!_isSaveDirectoryExplicitlySelected)
        {
            SaveDirectory = Path.GetDirectoryName(PmmPath) ?? "";
        }
    }

    private async void ReadLastFrame()
    {
        if(DispatcherQueue is null) { return; }
        if(Extractor is null) { return; }

        LastFrameCancellation?.Cancel();
        LastFrameCancellation = new();

        var lastFrame = await Extractor.CalcLastFrame(LastFrameCancellation.Token);
        DispatcherQueue.TryEnqueue(() => LastFrame = lastFrame);
    }

    [RelayCommand]
    private async Task ReadPmmAsync()
    {
        var pmmPath = (await StorageHelper.PickSingleFileAsync(".pmm"))?.Path;

        if (pmmPath is null) { return; }

        ReadPmm(pmmPath);
    }

    [RelayCommand]
    private async Task ReadSaveFolderAsync()
    {
        var folder = (await StorageHelper.PickFolderAsync())?.Path;

        if (folder is null) { return; }

        SaveDirectory = folder;
        _isSaveDirectoryExplicitlySelected = true;
    }

    [RelayCommand(CanExecute = nameof(CanExtractExecute))]
    public void Extract()
    {
        if(Extractor is null) { return; }
        if(SelectedPmmModels is null) { return; }

        if (DoesExtractCamera)
        {
            var options = new CameraMotionExtractionOptions
            {
                Camera = true,
                Light = DoesExtractLight,
                Shadow = false,
                StartFrame = StartFrame,
                EndFrame = EndFrame > StartFrame ? (uint)EndFrame : null,
            };

            Extractor.ExtractCameraMotion(SaveDirectory, options);
        }

        foreach (var model in SelectedPmmModels.Cast<PathInformation>().Select(obj => obj.Index))
        {
            var options = new ModelMotionExtractionOptions
            {
                Morph = DoesExtractMorph,
                Motion = DoesExtractMotion,
                Property = DoesExtractMotion,
                StartFrame = StartFrame,
                EndFrame = EndFrame > StartFrame ? (uint)EndFrame : null,
            };

            Extractor.ExtractModelMotion(model, SaveDirectory, options);
        }

        OnCompleted?.Invoke(this, new());
    }

    private bool CanExtractExecute() => !(
            string.IsNullOrWhiteSpace(PmmPath) ||
            string.IsNullOrWhiteSpace(SaveDirectory) ||
            Extractor is null
        ) && (
            (IsModelSelected && (DoesExtractMotion || DoesExtractMorph)) ||
            DoesExtractCamera
        );
}
