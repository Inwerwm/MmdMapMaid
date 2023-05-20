using MikuMikuMethods.Common;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Bezier;

namespace MmdMapMaid.Core.Models.Vmd;
public static class MorphInterpolater
{
    public static Point2<double> StartPoint => new(0, 0);
    public static Point2<double> EndPoint => new(1, 1);

    public static List<Point2<double>> CreateInterpolatedPoints(Point2<double> earlierControlPoint, Point2<double> laterControlPoint, int frameLength, double accuracy)
    {
        var xStep = 1.0 / frameLength;

        var sampledPoints = BezierCurve.SampleCubicBezierCurve(StartPoint, earlierControlPoint, laterControlPoint, EndPoint, xStep);
        return SimplifyPath.Simplify(sampledPoints, accuracy);
    }

    public static string? WriteVmd(string savePath, string? modelName, string morphName, int frameLength, IEnumerable<Point2<double>> weights, SaveOptions? options = null)
    {
        options ??= new();
        modelName ??= "InterpolatedMorphWeights";

        var vmd = new VocaloidMotionData()
        {
            ModelName = modelName,
            MorphFrames = weights.Select(point => new VmdMorphFrame(morphName)
            {
                Frame = (uint)Math.Round(point.X * frameLength),
                Weight = (float)point.Y,
            }).ToList()
        };

        return options.SaveWithBackupAndReturnCreatedPath(savePath, vmd.Write);
    }
}
