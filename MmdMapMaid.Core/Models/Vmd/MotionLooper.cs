using MikuMikuMethods;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Vmd.MotionLoop;

namespace MmdMapMaid.Core.Models.Vmd;
public class MotionLooper
{
    public DuplicationCounter DuplicationCounter
    {
        get;
    }
    public IntervalCalculator IntervalCalculator
    {
        get;
    }
    public FrameReprinter FrameReprinter
    {
        get;
    }

    public MotionLooper(DuplicationCounter? duplicationCounter = null, IntervalCalculator? intervalCalculator = null)
    {
        DuplicationCounter = duplicationCounter ?? new();
        IntervalCalculator = intervalCalculator ?? new(30);
        FrameReprinter = new();
    }

    public static VocaloidMotionData ReadFile(string filePath) =>
        !File.Exists(filePath) ? throw new FileNotFoundException() :
        Path.GetExtension(filePath).ToLower() != ".vmd" ? throw new InvalidDataException() :
                                                                new VocaloidMotionData(filePath);

    public VocaloidMotionData CreateLoopMotion(VocaloidMotionData vmd) =>
        FrameDuplicator.CreateLoopMotion(vmd, IntervalCalculator, DuplicationCounter);

    public void ReprintMorph(VocaloidMotionData source, VocaloidMotionData target)
    {
        if (source.CameraFrames.Any() && target.CameraFrames.Any())
        {
            FrameReprinter.ReprintFromNearest(source.CameraFrames, target.CameraFrames);
        }

        if (source.MotionFrames.Any() && target.MotionFrames.Any())
        {
            FrameReprinter.ReprintFromNearest(source.MotionFrames, target.MotionFrames);
        }
    }

    public VocaloidMotionData FollowPut(VocaloidMotionData source, string sourceItemName, VocaloidMotionData target) =>
        FrameReprinter.PutFromScore(source, sourceItemName, target);

    private static IEnumerable<string>? ExtractFrameNames(IEnumerable<IVmdFrame> frames) => frames.Any() ? frames.GroupBy(f => f.Name).Select(g => g.Key) : null;

    public IEnumerable<string> ExtractIncludedItemNames(VocaloidMotionData vmd) => vmd.Kind switch
    {
        VmdKind.Camera => ExtractFrameNames(vmd.GetAllFrames()) ?? Array.Empty<string>(),
        VmdKind.Model =>
            (
                ExtractFrameNames(vmd.MorphFrames) ?? Array.Empty<string>()
            ).Concat(
                ExtractFrameNames(vmd.MotionFrames)?.OrderBy(name => name, new BoneNameComparer()) as IEnumerable<string> ?? Array.Empty<string>()
            ),
        _ => throw new NotImplementedException(),
    };
}
