using System.Numerics;
using MikuMikuMethods.Extension;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Helpers;

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
                (IVmdFrame, IVmdFrame secondFrame) => secondFrame
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

    /// <summary>
    /// 生成されたフレームのシーケンスを作成し、それらを指定されたガイドフレームに沿って配置します。
    /// </summary>
    /// <param name="source">ソースとなるフレームのシーケンス。</param>
    /// <param name="guide">ガイドとなるフレームのシーケンス。</param>
    /// <returns>生成され、ガイドに沿って配置されたフレームのシーケンス。</returns>
    public static IEnumerable<IVmdFrame> GenerateAlignedFrames(IEnumerable<IVmdFrame> source, IEnumerable<IVmdFrame> guide) =>
        GenerateAlignedFrames(source, guide.OrderBy(frame => frame.Frame).ToList());

    /// <summary>
    /// ソースフレームをガイドフレームに沿って再配置します。
    /// </summary>
    /// <param name="source">ソースとなるフレームのシーケンス。</param>
    /// <param name="orderedGuide">ソートされたガイドフレームのリスト。</param>
    /// <returns>生成され、ガイドに沿って配置されたフレームのシーケンス。</returns>
    private static IEnumerable<IVmdFrame> GenerateAlignedFrames(IEnumerable<IVmdFrame> source, List<IVmdFrame> orderedGuide) => source
        .GroupBy(frame => frame.Name)
        .Select(group => ComputeOffsets(orderedGuide, group))
        .SelectMany(frameOffsets => frameOffsets.TakeCyclic()
            .Zip(orderedGuide, ComputeFrameTimes)
            .Select(CloneAndRepositionFrame)
        );

    /// <summary>
    /// ソースフレームとガイドフレームの間のオフセットを計算します。
    /// </summary>
    /// <param name="orderedGuide">ソートされたガイドフレームのリスト。</param>
    /// <param name="group">ソースとなるフレームのグループ。</param>
    /// <returns>フレームとそのオフセットのシーケンス。</returns>
    private static IVmdFrame CloneAndRepositionFrame((IVmdFrame Guide, IVmdFrame Frame, uint FrameTime) frame)
    {
        return (frame.Guide, frame.Frame) switch
        {
            (IVmdInterpolatable guideInterpolatable, VmdCameraFrame cameraClone) => new VmdCameraFrame(frame.FrameTime, guideInterpolatable.InterpolationCurves)
            {
                Distance = cameraClone.Distance,
                Position = cameraClone.Position,
                Rotation = cameraClone.Rotation,
                ViewAngle = cameraClone.ViewAngle,
                IsPerspectiveOff = cameraClone.IsPerspectiveOff,
            },
            (IVmdInterpolatable guideInterpolatable, VmdMotionFrame motionClone) => new VmdMotionFrame(motionClone.Name, frame.FrameTime, guideInterpolatable.InterpolationCurves)
            {
                Position = motionClone.Position,
                Rotation = motionClone.Rotation,
            },
            (IVmdFrame, IVmdFrame f) => CloneFrameWithNewTime(f, frame.FrameTime)
        };
    }

    /// <summary>
    /// 各フレームの新しいタイムスタンプを計算します。
    /// </summary>
    /// <param name="guideFrame">ガイドフレーム。</param>
    /// <param name="frameOffset">フレームとそのオフセット。</param>
    /// <returns>フレーム、ガイドフレーム、新しいタイムスタンプのタプル。</returns>
    private static (IVmdFrame Guide, IVmdFrame Frame, uint FrameTime) ComputeFrameTimes((IVmdFrame Frame, long Offset) frameOffset, IVmdFrame guideFrame)
    {
        return (
            Guide: guideFrame,
            frameOffset.Frame,
            FrameTime: (uint)Math.Max(guideFrame.Frame + frameOffset.Offset, 0)
        );
    }

    /// <summary>
    /// フレームをクローンし、新しいタイムスタンプで配置します。
    /// </summary>
    /// <param name="frame">フレーム、ガイドフレーム、新しいタイムスタンプのタプル。</param>
    /// <returns>新しいタイムスタンプで再配置されたクローンフレーム。</returns>
    private static IEnumerable<(IVmdFrame Frame, long Offset)> ComputeOffsets(List<IVmdFrame> orderedGuide, IGrouping<string, IVmdFrame> group)
    {
        return group
            .OrderBy(frame => frame.Frame)
            .Zip(orderedGuide, (groupFrame, guideFrame) => (
                Frame: groupFrame,
                Offset: (long)groupFrame.Frame - guideFrame.Frame
            ));
    }

    /// <summary>
    /// フレームをクローンし、新しいタイムスタンプで配置します。
    /// </summary>
    /// <param name="frame">クローンするフレーム。</param>
    /// <param name="frameTime">新しいタイムスタンプ。</param>
    /// <returns>新しいタイムスタンプで再配置されたクローンフレーム。</returns>
    private static IVmdFrame CloneFrameWithNewTime(IVmdFrame frame, uint frameTime)
    {
        var clone = (IVmdFrame)frame.Clone();
        clone.Frame = frameTime;
        return clone;
    }
}