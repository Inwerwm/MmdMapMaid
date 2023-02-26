using System.Security.Cryptography;
using MikuMikuMethods;
using MikuMikuMethods.Vmd;

namespace MmdMapMaid.Core.Models.Vmd.MotionLoop;
public class FrameReprinter
{
    /// <summary>
    /// ターゲットフレームの補間曲線をソースの最近傍フレームから転写する
    /// <para>破壊的</para>
    /// </summary>
    /// <typeparam name="T">フレームの型</typeparam>
    /// <param name="sourceFrames">転写元のフレーム</param>
    /// <param name="targetFrames">転写先のフレーム</param>
    public static void ReprintFromNearest<T>(IEnumerable<T> sourceFrames, List<T> targetFrames) where T : class, IVmdInterpolatable, IVmdFrame =>
        targetFrames.AsParallel().ForAll(target =>
        {
            var nearest = sourceFrames.MinBy(source => Math.Abs(source.Frame - target.Frame));
            if (nearest is not null)
            {
                InterpolationCurve.CopyCurves(nearest.InterpolationCurves, target.InterpolationCurves);
            }
        });

    /// <summary>
    /// ソースVMDのフレーム位置の場所にターゲットフレームを設置したVMDを返す
    /// <para>ターゲットのフレーム数がソースより少なければループする</para>
    /// <para>補間曲線のあるフレームの場合はそれも転写する</para>
    /// </summary>
    /// <param name="source">位置と補間曲線の基VMD</param>
    /// <param name="sourceItemName">ソース側の位置と補間曲線を取得する項目名</param>
    /// <param name="target">複製するフレームの入ったVMD</param>
    /// <returns>ソースVMDのフレーム位置の場所にターゲットフレームを設置したVMD</returns>
    public static VocaloidMotionData PutFromScore(VocaloidMotionData source, string sourceItemName, VocaloidMotionData target)
    {
        var sourceFrames = source.GetAllFrames().Where(f => f.Name == sourceItemName).OrderBy(f => f.Frame);
        var targetItems = target.GetAllFrames().GroupBy(f => f.Name).ToArray();

        var result = new VocaloidMotionData() { ModelName = target.ModelName };

        foreach (var currentTargetItem in targetItems)
        {
            var sourceQueue = new Queue<IVmdFrame>(sourceFrames);
            while (sourceQueue.Any())
            {
                foreach (var currentTargetFrame in currentTargetItem)
                {
                    if (!sourceQueue.Any())
                    {
                        break;
                    }

                    var currentSourceFrame = sourceQueue.Dequeue();
                    var currentResultFrame = (IVmdFrame)currentTargetFrame.Clone();

                    // フレーム位置を転写
                    currentResultFrame.Frame = currentSourceFrame.Frame;

                    // 補間曲線を持つフレームであればそれも転写する
                    if (isInterpolatable(currentSourceFrame) && isInterpolatable(currentResultFrame))
                    {
                        InterpolationCurve.CopyCurves(((IVmdInterpolatable)currentSourceFrame).InterpolationCurves, ((IVmdInterpolatable)currentResultFrame).InterpolationCurves);
                    }

                    result.AddFrame(currentResultFrame);
                }
            }
        }
        return result;

        static bool isInterpolatable(IVmdFrame frame) => frame.GetType().GetInterfaces()?.Contains(typeof(IVmdInterpolatable)) ?? false;
    }
}
