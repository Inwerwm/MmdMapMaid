namespace MmdMapMaid.Core.Models.Vmd.MotionLoop;
public class IntervalCalculator
{
    private double? bpm;
    private double? interval;

    /// <summary>
    /// BPMと設置間隔の相互計算に必要なキーフレームの設置フレームレート
    /// MMDでは30
    /// </summary>
    public double BaseFrameRate
    {
        get; init;
    }

    public double? BPM
    {
        get => bpm;
        set
        {
            var val = FilterPositiveOnly(value);

            bpm = val;
            var beetPerSecond = val / 60;
            interval = 30 / beetPerSecond;
        }
    }

    public double? Interval
    {
        get => interval;
        set
        {
            var val = FilterPositiveOnly(value);

            interval = val;
            var bps = 30 / val;
            bpm = bps * 60;
        }
    }

    public IntervalCalculator(double baseFrameRate, double? bpm = 120, double? interval = 15)
    {
        BaseFrameRate = baseFrameRate;

        this.bpm = bpm;
        this.interval = interval;
    }

    private static double? FilterPositiveOnly(double? value) =>
        value.HasValue && value > 0 ? value : null;
}
