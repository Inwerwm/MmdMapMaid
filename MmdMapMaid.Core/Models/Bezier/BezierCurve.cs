using MikuMikuMethods.Common;

namespace MmdMapMaid.Core.Models.Bezier;
public class BezierCurve
{
    public static Point2<double> CalculateCubicBezierPoint(double t, Point2<double> p0, Point2<double> p1, Point2<double> p2, Point2<double> p3)
    {
        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var uuu = uu * u;
        var ttt = tt * t;

        var point = new Point2<double>(uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X,
                              uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y);

        return point;
    }

    public static List<Point2<double>> SampleCubicBezierCurve(Point2<double> p0, Point2<double> p1, Point2<double> p2, Point2<double> p3, int divisionCount, double precision)
    {
        // 分割したx座標の範囲を計算
        var xMin = p0.X;
        var xMax = p3.X;
        var xStep = (xMax - xMin) / divisionCount;

        // divisionCountから2を引くことで最初と最後の点を除外
        var range = Enumerable.Range(1, divisionCount - 1);

        var sampledPoints = range.AsParallel()
            .Select(i => xMin + i * xStep)
            .Select(x => BinarySearchForT(p0, p1, p2, p3, x, precision))
            .Select(t => CalculateCubicBezierPoint(t, p0, p1, p2, p3))
            .Prepend(p0)
            .Append(p3)
            .OrderBy(point => point.X)
            .ToList();

        return sampledPoints;
    }

    private static double BinarySearchForT(Point2<double> p0, Point2<double> p1, Point2<double> p2, Point2<double> p3, double x, double precision)
    {
        double tLow = 0, tHigh = 1;
        double tMid = 0;

        while (tHigh - tLow > precision)
        {
            tMid = (tLow + tHigh) / 2;
            var point = CalculateCubicBezierPoint(tMid, p0, p1, p2, p3);

            if (point.X > x)
                tHigh = tMid;
            else
                tLow = tMid;
        }

        return tMid;
    }


}
