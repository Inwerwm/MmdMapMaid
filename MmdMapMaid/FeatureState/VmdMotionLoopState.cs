using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Core.Models;
using MmdMapMaid.Core.Models.Vmd;

namespace MmdMapMaid.FeatureState;

internal partial class VmdMotionLoopState : ObservableObject
{
    private const string SettingsKeyOfElementVmdPath = "LoopMotionElementVmdPath";
    private const string SettingsKeyOfMotionLooper = "MotionLooper";

    ILocalSettingsService _localSettingsService;

    private MotionLooper MotionLooper
    {
        get;
    } = new();

    private bool IsIntervalUpdating
    {
        get;
        set;
    } = false;

    [ObservableProperty]
    private string _elementVmdPath;

    [ObservableProperty]
    private double _bpm;

    [ObservableProperty]
    private double _interval;

    [ObservableProperty]
    private int _frequency;

    [ObservableProperty]
    private int _beat;

    [ObservableProperty]
    private int _loopCount;

    [ObservableProperty]
    private int _plotCountOffset;

    [ObservableProperty]
    private int _plotCount;

    public VmdMotionLoopState(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        _elementVmdPath = _localSettingsService.ReadSetting<string>(SettingsKeyOfElementVmdPath) ?? string.Empty;

        var settings = _localSettingsService.ReadSetting<MotionLooper>(SettingsKeyOfMotionLooper) ?? null;

        Bpm = settings?.IntervalCalculator.BPM ?? 120;
        MotionLooper.IntervalCalculator.BPM = Bpm;

        Interval = MotionLooper.IntervalCalculator.Interval ?? 1;

        Frequency = settings?.DuplicationCounter.Frequency ?? 1;
        Beat = settings?.DuplicationCounter.Beat ?? 4;
        LoopCount = settings?.DuplicationCounter.LoopCount ?? 4;
        PlotCountOffset = settings?.DuplicationCounter.CountOffset ?? 0;

        MotionLooper.DuplicationCounter.Frequency = Frequency;
        MotionLooper.DuplicationCounter.Beat = Beat;
        MotionLooper.DuplicationCounter.LoopCount = LoopCount;
        MotionLooper.DuplicationCounter.CountOffset = PlotCountOffset;
        PlotCount = MotionLooper.DuplicationCounter.ElementCount;

        PropertyChanged += VmdMotionLoopState_PropertyChanged;
    }

    private void VmdMotionLoopState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ElementVmdPath):
                _localSettingsService.SaveSetting(SettingsKeyOfElementVmdPath, ElementVmdPath);
                break;
            case nameof(Bpm):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                MotionLooper.IntervalCalculator.BPM = Bpm;
                Interval = MotionLooper.IntervalCalculator.Interval ?? 1;

                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);

                IsIntervalUpdating = false;
                break;
            case nameof(Interval):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                MotionLooper.IntervalCalculator.Interval = Interval;
                Bpm = MotionLooper.IntervalCalculator.BPM ?? 1;

                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);

                IsIntervalUpdating = false;
                break;
            case nameof(Frequency):
                MotionLooper.DuplicationCounter.Frequency = Frequency;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);
                break;
            case nameof(Beat):
                MotionLooper.DuplicationCounter.Beat = Beat;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);
                break;
            case nameof(LoopCount):
                MotionLooper.DuplicationCounter.LoopCount = LoopCount;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);
                break;
            case nameof(PlotCountOffset):
                MotionLooper.DuplicationCounter.CountOffset = PlotCountOffset;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                _localSettingsService.SaveSetting(SettingsKeyOfMotionLooper, MotionLooper);
                break;
        }
    }

    public string Execute()
    {
        var elementVmd = MotionLooper.ReadFile(ElementVmdPath);
        var loopMotion = MotionLooper.CreateLoopMotion(elementVmd);

        var save = new SaveOptions(EnableBackup: false);
        var savePath = SaveOptions.AddSuffixTo(ElementVmdPath, "_loop");
        save.SaveWithBackupAndReturnCreatedPath(savePath, loopMotion.Write);

        return $$"""
        総数: {{elementVmd.GetAllFrames().Count()}} → {{loopMotion.GetAllFrames().Count()}}
        出力: {{savePath}}{{Environment.NewLine}}
        """;
    }
}
