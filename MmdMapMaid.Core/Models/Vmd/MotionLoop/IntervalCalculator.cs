namespace MmdMapMaid.Core.Models.Vmd.MotionLoop;
internal class IntervalCalculator
{
    private decimal? bpm;
    private decimal? interval;

    /// <summary>
    /// BPMと設置間隔の相互計算に必要なキーフレームの設置フレームレート
    /// MMDでは30
    /// </summary>
    public decimal BaseFrameRate
    {
        get; init;
    }

    public decimal? BPM
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

    public decimal? Interval
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

    public IntervalCalculator(decimal baseFrameRate)
    {
        BaseFrameRate = baseFrameRate;
    }

    private static decimal? FilterPositiveOnly(decimal? value) =>
        value.HasValue && value > 0m ? value : null;
}
