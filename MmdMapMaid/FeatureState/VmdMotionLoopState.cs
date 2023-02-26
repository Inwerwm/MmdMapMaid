using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Core.Models.Vmd.MotionLoop;
using Windows.Storage;

namespace MmdMapMaid.FeatureState;

internal partial class VmdMotionLoopState : ObservableObject
{
    private IntervalCalculator IntervalCalculator
    {
        get;
    } = new(30);

    private DuplicationCounter DuplicationCounter
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

    [ObservableProperty]
    private string _log;

    public VmdMotionLoopState()
    {
        _elementVmdPath = Properties.Settings.Default.ElementVmdPath;
        Bpm = Properties.Settings.Default.Bpm;
        Interval = Properties.Settings.Default.Interval;
        Frequency = Properties.Settings.Default.Frequency;
        Beat = Properties.Settings.Default.Beat;
        LoopCount = Properties.Settings.Default.LoopCount;
        PlotCountOffset = Properties.Settings.Default.PlotCountOffset;
        
        DuplicationCounter.Frequency = Frequency;
        DuplicationCounter.Beat = Beat;
        DuplicationCounter.LoopCount = LoopCount;
        DuplicationCounter.CountOffset = PlotCountOffset;
        PlotCount = DuplicationCounter.ElementCount;

        _log = string.Empty;

        PropertyChanged += VmdMotionLoopState_PropertyChanged;
    }

    private void VmdMotionLoopState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Bpm):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                IntervalCalculator.BPM = Bpm;
                Interval = IntervalCalculator.Interval ?? 1;

                Properties.Settings.Default.Interval = Interval;
                Properties.Settings.Default.Bpm = Bpm;

                IsIntervalUpdating = false;
                break;
            case nameof(Interval):
                if (IsIntervalUpdating) { break; }
                IsIntervalUpdating = true;

                IntervalCalculator.Interval = Interval;
                Bpm = IntervalCalculator.BPM ?? 1;

                Properties.Settings.Default.Interval = Interval;
                Properties.Settings.Default.Bpm = Bpm;

                IsIntervalUpdating = false;
                break;
            case nameof(Frequency):
                DuplicationCounter.Frequency = Frequency;
                PlotCount = DuplicationCounter.ElementCount;
                Properties.Settings.Default.Frequency = Frequency;
                break;
            case nameof(Beat):
                DuplicationCounter.Beat = Beat;
                PlotCount = DuplicationCounter.ElementCount;
                Properties.Settings.Default.Beat = Beat;
                break;
            case nameof(LoopCount):
                DuplicationCounter.LoopCount = LoopCount;
                PlotCount = DuplicationCounter.ElementCount;
                Properties.Settings.Default.LoopCount = LoopCount;
                break;
            case nameof(PlotCountOffset):
                DuplicationCounter.CountOffset = PlotCountOffset;
                PlotCount = DuplicationCounter.ElementCount;
                Properties.Settings.Default.PlotCountOffset = PlotCountOffset;
                break;
        }

        Properties.Settings.Default.Save();
    }
}
