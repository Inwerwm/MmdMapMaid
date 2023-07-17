using MikuMikuMethods.Common;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Bezier;

namespace MmdMapMaid.Core.Models.Vmd;
public static class MorphInterpolater
{
    public static Point2<double> StartPoint => new(0, 0);
    public static Point2<double> EndPoint => new(1, 1);

    public static List<Point2<double>> CreateInterpolatedPoints(Point2<double> earlierControlPoint, Point2<double> laterControlPoint, int frameLength, double accuracy, double precision = 0.001)
    {
        var sampledPoints = BezierCurve.SampleCubicBezierCurve(StartPoint, earlierControlPoint, laterControlPoint, EndPoint, frameLength, precision);
        return SimplifyPath.Simplify(sampledPoints, accuracy);
    }

    private static double ScaleWeight(double weight, double start, double end) => (weight * (end - start)) + start;

    public static string? WriteVmd(string savePath, string? modelName, string morphName, int frameLength, IEnumerable<Point2<double>> weights, double startWeight, double endWeight, SaveOperations? options = null)
    {
        options ??= new();
        modelName ??= "InterpolatedMorphWeights";

        var vmd = new VocaloidMotionData()
        {
            ModelName = modelName,
            MorphFrames = weights.Select(point => new VmdMorphFrame(morphName)
                {
                    Frame = (uint)Math.Round(point.X * frameLength),
                    Weight = (float)ScaleWeight(point.Y, startWeight, endWeight),
                }).ToList()
        };

        return options.SaveAndBackupFile(savePath, vmd.Write);
    }
}
