using MikuMikuMethods.Vmd;

namespace MmdMapMaid.Core.Models.Vmd.MotionLoop;
internal static class FrameDuplicator
{
    public static IEnumerable<IVmdFrame> Duplicate(IEnumerable<IVmdFrame> frames, uint offset) =>
        frames.Select(frame =>
        {
            var cloneFrame = frame.Clone() as IVmdFrame;
            if (cloneFrame != null)
            {
                cloneFrame.Frame += offset;
            }

            return cloneFrame;
        }).Where(frame => frame != null).Cast<IVmdFrame>();

    public static IEnumerable<IVmdFrame> Duplicate(IEnumerable<IVmdFrame> frames, decimal interval, int count) =>
        Enumerable.Range(0, count).SelectMany(i => Duplicate(frames, (uint)Math.Round(interval * i)));

    public static VocaloidMotionData CreateLoopMotion(VocaloidMotionData vmd, IntervalCalculator loop, DuplicationCounter counter)
    {
        if (loop.Interval is null)
        {
            throw new ArgumentNullException("loop.Interval", "設置間隔が未設定です。");
        }

        var result = new VocaloidMotionData()
        {
            ModelName = vmd.ModelName,
        };

        var interval = loop.Interval.Value * counter.Frequency;
        var count = counter.ElementCount;

        void CreateAndAddDuplicate<T>(List<T> source, List<T> result) where T : IVmdFrame
        {
            if (source.Any())
            {
                result.AddRange(FrameDuplicator.Duplicate(source.Select(f => (IVmdFrame)f), interval, count).Select(f => (T)f));
            }
        }

        CreateAndAddDuplicate(vmd.CameraFrames, result.CameraFrames);
        CreateAndAddDuplicate(vmd.LightFrames, result.LightFrames);
        CreateAndAddDuplicate(vmd.MorphFrames, result.MorphFrames);
        CreateAndAddDuplicate(vmd.MotionFrames, result.MotionFrames);
        CreateAndAddDuplicate(vmd.PropertyFrames, result.PropertyFrames);
        CreateAndAddDuplicate(vmd.ShadowFrames, result.ShadowFrames);

        return result;
    }
}
