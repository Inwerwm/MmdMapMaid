using MikuMikuMethods.Common;

namespace MmdMapMaid.Core.Models.Bezier;
public class SimplifyPath
{
    private static double MidPointLineDistance(Point2<double> start, Point2<double> mid, Point2<double> end)
    {
        // 最初と最後の点間の直線上の真ん中の点の y 座標を求める
        var interpolatedY = start.Y + ((end.Y - start.Y) * ((mid.X - start.X) / (end.X - start.X)));

        // 真ん中の点との y 座標の差の絶対値を返す
        return Math.Abs(mid.Y - interpolatedY);
    }

    public static List<Point2<double>> Simplify(List<Point2<double>> points, double approximationAccuracy) =>
        points
            .Skip(1)
            .Take(points.Count - 2)
            .Select((p, index) => (point: p, distance: MidPointLineDistance(points[index], p, points[index + 2])))
            .Where(p => p.distance > approximationAccuracy)
            .Select(p => p.point)
            .Prepend(points.First())
            .Append(points.Last())
            .ToList();
}
