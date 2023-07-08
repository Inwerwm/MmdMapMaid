using System.Numerics;
using MikuMikuMethods.Extension;
using MikuMikuMethods.Vmd;

namespace MmdMapMaid.Core.Models.Vmd;
public static class VmdRangeEditor
{
    public static IEnumerable<IVmdFrame> ScaleOffset(IEnumerable<IVmdFrame> frames, float scale) =>
        TakeFirstTwoFramesPerBone(frames).SelectMany(frames => new[]
        {
            frames[0],
            (frames[0], frames[1]) switch
            {
                (VmdMotionFrame motionFirstFrame, VmdMotionFrame motionSecondFrame) =>
                    new VmdMotionFrame(motionSecondFrame.Name, motionSecondFrame.Frame, motionSecondFrame.InterpolationCurves)
                    {
                        Position = motionFirstFrame.Position + scale * (motionSecondFrame.Position - motionFirstFrame.Position),
                        Rotation = Quaternion.Lerp(motionFirstFrame.Rotation, motionSecondFrame.Rotation, scale)
                    },
                (VmdCameraFrame cameraFirstFrame, VmdCameraFrame cameraSecondFrame) =>
                    new VmdCameraFrame(cameraSecondFrame.Frame, cameraSecondFrame.InterpolationCurves)
                    {
                        Position = cameraFirstFrame.Position + scale * (cameraSecondFrame.Position - cameraFirstFrame.Position),
                        Rotation = cameraFirstFrame.Rotation + scale * (cameraSecondFrame.Rotation - cameraFirstFrame.Rotation)
                    },
                _ => throw new NotImplementedException()
            }
        });

    private static IEnumerable<T[]> TakeFirstTwoFramesPerBone<T>(IEnumerable<T> motionFrames) where T : IVmdFrame => motionFrames
        .GroupBy(frame => frame.Name)
        .Select(group => group
            .OrderBy(frame => frame.Frame)
            .Take(2)
            .ToArray()
        )
        .Where(frames => frames.Length == 2);
}
