using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models;
using MmdMapMaid.Core.Models.Vmd;

namespace MmdMapMaid.FeatureState;

internal partial class VmdMotionLoopState : ObservableObject
{
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

    public VmdMotionLoopState()
    {
        _elementVmdPath = Properties.Settings.Default.ElementVmdPath;

        Bpm = Properties.Settings.Default.Bpm;
        MotionLooper.IntervalCalculator.BPM = Bpm;

        Interval = MotionLooper.IntervalCalculator.Interval ?? 1;

        Frequency = Properties.Settings.Default.Frequency;
        Beat = Properties.Settings.Default.Beat;
        LoopCount = Properties.Settings.Default.LoopCount;
        PlotCountOffset = Properties.Settings.Default.PlotCountOffset;

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
                Properties.Settings.Default.ElementVmdPath = ElementVmdPath;
                break;
            case nameof(Bpm):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                MotionLooper.IntervalCalculator.BPM = Bpm;
                Interval = MotionLooper.IntervalCalculator.Interval ?? 1;

                Properties.Settings.Default.Bpm = Bpm;

                IsIntervalUpdating = false;
                break;
            case nameof(Interval):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                MotionLooper.IntervalCalculator.Interval = Interval;
                Bpm = MotionLooper.IntervalCalculator.BPM ?? 1;

                Properties.Settings.Default.Bpm = Bpm;

                IsIntervalUpdating = false;
                break;
            case nameof(Frequency):
                MotionLooper.DuplicationCounter.Frequency = Frequency;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                Properties.Settings.Default.Frequency = Frequency;
                break;
            case nameof(Beat):
                MotionLooper.DuplicationCounter.Beat = Beat;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                Properties.Settings.Default.Beat = Beat;
                break;
            case nameof(LoopCount):
                MotionLooper.DuplicationCounter.LoopCount = LoopCount;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                Properties.Settings.Default.LoopCount = LoopCount;
                break;
            case nameof(PlotCountOffset):
                MotionLooper.DuplicationCounter.CountOffset = PlotCountOffset;
                PlotCount = MotionLooper.DuplicationCounter.ElementCount;
                Properties.Settings.Default.PlotCountOffset = PlotCountOffset;
                break;
        }

        Properties.Settings.Default.Save();
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
