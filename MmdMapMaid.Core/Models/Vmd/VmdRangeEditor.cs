using MikuMikuMethods.Extension;
using MikuMikuMethods.Vmd;

namespace MmdMapMaid.Core.Models.Vmd;
public static class VmdRangeEditor
{
    public static IEnumerable<VmdMotionFrame> ScaleOffset(IEnumerable<VmdMotionFrame> motionFrames, float scale) =>
        ExcludeRanges(motionFrames).SelectMany(frames => new[] {
            frames[0],
            new(frames[1].Name, frames[1].Frame, frames[1].InterpolationCurves)
            {
                Position = frames[0].Position + scale * (frames[1].Position - frames[0].Position),
                Rotation = frames[0].Rotation.Scale(frames[1].Rotation, scale)
            }
        });

    public static IEnumerable<VmdCameraFrame> ScaleOffset(IEnumerable<VmdCameraFrame> motionFrames, float scale) =>
        ExcludeRanges(motionFrames).SelectMany(frames => new[] {
            frames[0],
            new(frames[1].Frame, frames[1].InterpolationCurves)
            {
                Position = frames[0].Position + scale * (frames[1].Position - frames[0].Position),
                Rotation = frames[0].Rotation + scale * (frames[1].Rotation - frames[0].Rotation)
            }
        });

    private static IEnumerable<T[]> ExcludeRanges<T>(IEnumerable<T> motionFrames)where T : IVmdFrame => motionFrames
        .GroupBy(frame => frame.Name)
        .Select(group =>
            group
                .OrderBy(frame => frame.Frame)
                .Take(2)
                .ToArray()
        )
        .Where(frames => frames.Length == 2);
}
