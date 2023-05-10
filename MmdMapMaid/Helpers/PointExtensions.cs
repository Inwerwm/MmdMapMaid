using MikuMikuMethods.Common;
using Windows.Foundation;

namespace MmdMapMaid.Helpers;
internal static class PointExtensions
{
    public static Point ToPoint(this Point2<double> point) => new(point.X, point.Y);
    public static Point2<double> ToPoint2(this Point point) => new(point.X, point.Y);
}
