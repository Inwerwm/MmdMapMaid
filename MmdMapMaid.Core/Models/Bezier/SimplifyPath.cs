using MikuMikuMethods.Common;

namespace MmdMapMaid.Core.Models.Bezier;
public class SimplifyPath
{
    private static double TriangleArea(Point2<double> a, Point2<double> b, Point2<double> c) =>
        Math.Abs(((a.X * (b.Y - c.Y)) + (b.X * (c.Y - a.Y)) + (c.X * (a.Y - b.Y))) / 2.0);

    private static double NormalizedTriangleArea(Point2<double> a, Point2<double> b, Point2<double> c)
    {
        // バウンディングボックスの計算
        var minX = Math.Min(Math.Min(a.X, b.X), c.X);
        var minY = Math.Min(Math.Min(a.Y, b.Y), c.Y);
        var maxX = Math.Max(Math.Max(a.X, b.X), c.X);
        var maxY = Math.Max(Math.Max(a.Y, b.Y), c.Y);

        // 座標の正規化
        var normalizedA = new Point2<double>((a.X - minX) / (maxX - minX), (a.Y - minY) / (maxY - minY));
        var normalizedB = new Point2<double>((b.X - minX) / (maxX - minX), (b.Y - minY) / (maxY - minY));
        var normalizedC = new Point2<double>((c.X - minX) / (maxX - minX), (c.Y - minY) / (maxY - minY));

        // 正規化された座標を用いて面積を計算
        var normalizedArea = Math.Abs(((normalizedA.X * (normalizedB.Y - normalizedC.Y)) +
                                        (normalizedB.X * (normalizedC.Y - normalizedA.Y)) +
                                        (normalizedC.X * (normalizedA.Y - normalizedB.Y))) / 2.0);

        return normalizedArea;
    }


    public static List<Point2<double>> Simplify(List<Point2<double>> points, double approximationAccuracy) =>
        points
            .Skip(1)
            .Take(points.Count - 2)
            .Select((p, index) => (point: p, area: NormalizedTriangleArea(points[index], p, points[index + 2])))
            .Where(p => p.area > approximationAccuracy)
            .Select(p => p.point)
            .Prepend(points.First())
            .Append(points.Last())
            .ToList();
}
