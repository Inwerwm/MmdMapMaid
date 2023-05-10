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

    public static List<Point2<double>> SampleCubicBezierCurve(Point2<double> p0, Point2<double> p1, Point2<double> p2, Point2<double> p3, double xStep, double precision = 0.01)
    {
        var sampledPoints = new List<Point2<double>>();
        double t = 0;
        var currentX = p0.X;

        while (t <= 1.0)
        {
            var point = CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            if (System.Math.Abs(point.X - currentX) >= xStep)
            {
                sampledPoints.Add(point);
                currentX = point.X;
            }

            t += precision;
        }

        return sampledPoints;
    }
}
