using MikuMikuMethods.Common;

namespace MmdMapMaid.Core.Models.Bezier;
public class SimplifyPath
{
    private static double PointLineDistance(Point2<double> point, Point2<double> lineStart, Point2<double> lineEnd)
    {
        var lineDirection = new Point2<double>(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
        var pointDirection = new Point2<double>(point.X - lineStart.X, point.Y - lineStart.Y);

        var projectedLength = pointDirection.X * lineDirection.X + pointDirection.Y * lineDirection.Y;
        var projectedPoint = new Point2<double>(lineStart.X + (lineDirection.X * projectedLength), lineStart.Y + (lineDirection.Y * projectedLength));

        return Math.Sqrt((point.X - projectedPoint.X) * (point.X - projectedPoint.X) + (point.Y - projectedPoint.Y) * (point.Y - projectedPoint.Y));
    }

    private static void DouglasPeucker(List<Point2<double>> points, int startIndex, int endIndex, double epsilon, List<Point2<double>> simplifiedPoints)
    {
        double maxDistance = 0;
        var maxIndex = 0;

        for (var i = startIndex + 1; i < endIndex; i++)
        {
            var distance = PointLineDistance(points[i], points[startIndex], points[endIndex]);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }

        if (maxDistance > epsilon)
        {
            DouglasPeucker(points, startIndex, maxIndex, epsilon, simplifiedPoints);
            DouglasPeucker(points, maxIndex, endIndex, epsilon, simplifiedPoints);
        }
        else
        {
            simplifiedPoints.Add(points[startIndex]);
        }
    }

    public static List<Point2<double>> Simplify(List<Point2<double>> points, double approximationAccuracy)
    {
        if (points.Count < 3)
        {
            return points;
        }

        var simplifiedPoints = new List<Point2<double>>();
        DouglasPeucker(points, 0, points.Count - 1, approximationAccuracy, simplifiedPoints);
        simplifiedPoints.Add(points[^1]);

        return simplifiedPoints;
    }
}
