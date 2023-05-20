using MikuMikuMethods.Common;

namespace MmdMapMaid.Core.Models.Bezier;
public class SimplifyPath
{
    private static double TriangleArea(Point2<double> a, Point2<double> b, Point2<double> c) =>
        Math.Abs(((a.X * (b.Y - c.Y)) + (b.X * (c.Y - a.Y)) + (c.X * (a.Y - b.Y))) / 2.0);

    public static List<Point2<double>> Simplify(List<Point2<double>> points, double approximationAccuracy) =>
        SimplifyRecursive(points, approximationAccuracy);

    private static List<Point2<double>> SimplifyRecursive(List<Point2<double>> points, double approximationAccuracy)
    {
        if (points.Count < 3)
        {
            return points;
        }

        var simplifiedPoints = points
            .Skip(1)
            .Take(points.Count - 2)
            .Select((p, index) => (point: p, area: TriangleArea(points[index], p, points[index + 2])))
            .Where(p => p.area > approximationAccuracy)
            .Select(p => p.point)
            .Prepend(points.First())
            .Append(points.Last())
            .ToList();

        return (simplifiedPoints.Count == points.Count)
            ? simplifiedPoints
            : SimplifyRecursive(simplifiedPoints, approximationAccuracy);
    }
}
