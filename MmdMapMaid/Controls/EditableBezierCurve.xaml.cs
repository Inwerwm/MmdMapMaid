using System.Reflection.Metadata;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MmdMapMaid.Controls;
public sealed partial class EditableBezierCurve : UserControl
{
    public static readonly DependencyProperty EarlierControlPointProperty =
        DependencyProperty.Register(
            nameof(EarlierControlPoint),
            typeof(Point),
            typeof(EditableBezierCurve),
            new PropertyMetadata(default(Point), EarlierControlPointPropertyChanged));

    public static readonly DependencyProperty LaterControlPointProperty =
        DependencyProperty.Register(
            nameof(LaterControlPoint),
            typeof(Point),
            typeof(EditableBezierCurve),
            new PropertyMetadata(default(Point), LaterControlPointPropertyChanged));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(EditableBezierCurve),
            new PropertyMetadata(default(double), SizePropertyChanged));

    public Point EarlierControlPoint
    {
        get => (Point)GetValue(EarlierControlPointProperty);
        set => SetValue(EarlierControlPointProperty, value);
    }

    public Point LaterControlPoint
    {
        get => (Point)GetValue(LaterControlPointProperty);
        set => SetValue(LaterControlPointProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private static void EarlierControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {

        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);

        instance.CurveCanvas.Children.Remove(instance.EarlierLine);
        instance.EarlierLine = instance.DrawLine(instance.StartPoint, instance.EarlierControlPoint);

        instance.SetHandlePosition(instance.EarlierControlPointHandle, instance.EarlierControlPoint);
    }

    private static void LaterControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);

        instance.CurveCanvas.Children.Remove(instance.LaterLine);
        instance.LaterLine = instance.DrawLine(instance.LaterControlPoint, instance.EndPoint);

        instance.SetHandlePosition(instance.LaterControlPointHandle, instance.LaterControlPoint);
    }

    private static void SizePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        var newSize = (double)dependencyPropertyChangedEventArgs.NewValue;
        var oldValue = (double)dependencyPropertyChangedEventArgs.OldValue;
        var scale = oldValue == 0 ? 1 : newSize / oldValue;

        instance.CurveCanvas.Width = newSize;
        instance.CurveCanvas.Height = newSize;

        instance.EndPoint = new Point(newSize, newSize);

        instance.EarlierControlPoint = mul(instance.EarlierControlPoint, scale);
        instance.LaterControlPoint = mul(instance.LaterControlPoint, scale);

        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);

        instance.CurveCanvas.Children.Remove(instance.EarlierLine);
        instance.CurveCanvas.Children.Remove(instance.LaterLine);

        instance.EarlierLine = instance.DrawLine(instance.StartPoint, instance.EarlierControlPoint);
        instance.LaterLine = instance.DrawLine(instance.LaterControlPoint, instance.EndPoint);
    }

    private static Point mul(Point point, double scale) => new(point.X * scale, point.Y * scale);

    private Point StartPoint
    {
        get;
    }

    private Point EndPoint
    {
        get;
        set;
    }

    private Line EarlierLine
    {
        get;
        set;
    }

    private Line LaterLine
    {
        get;
        set;
    }

    public EditableBezierCurve()
    {
        InitializeComponent();

        Size = 1000;

        StartPoint = new Point(0, 0);
        EndPoint = new Point(Size, Size);

        EarlierControlPoint = new Point(250, 250);
        LaterControlPoint = new Point(750, 750);

        DrawCubicBezier(StartPoint, EarlierControlPoint, LaterControlPoint, EndPoint);

        SetHandlePosition(EarlierControlPointHandle, EarlierControlPoint);
        SetHandlePosition(LaterControlPointHandle, LaterControlPoint);

        AddDragHandlers(EarlierControlPointHandle, p => EarlierControlPoint = p);
        AddDragHandlers(LaterControlPointHandle, p => LaterControlPoint = p);
    }

    private void DrawCubicBezier(Point p0, Point p1, Point p2, Point p3)
    {
        var pathFigure = new PathFigure { StartPoint = p0 };
        pathFigure.Segments.Add(new BezierSegment
        {
            Point1 = p1,
            Point2 = p2,
            Point3 = p3,
        });

        var pathGeometry = new PathGeometry();
        pathGeometry.Figures.Add(pathFigure);

        BezierPath.Data = pathGeometry;
    }

    private Line DrawLine(Point start, Point end)
    {
        var line = new Line
        {
            X1 = start.X,
            Y1 = start.Y,
            X2 = end.X,
            Y2 = end.Y,
            Stroke = new SolidColorBrush(Colors.Black),
            StrokeThickness = 2
        };

        CurveCanvas.Children.Add(line);

        return line;
    }

    private void SetHandlePosition(Ellipse handle, Point position)
    {
        Canvas.SetLeft(handle, position.X - handle.Width / 2);
        Canvas.SetTop(handle, position.Y - handle.Height / 2);
    }

    private void AddDragHandlers(Ellipse handle, Action<Point> onDrag)
    {
        var isDragging = false;
        var lastPosition = new Point();

        handle.PointerPressed += (s, e) =>
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                isDragging = true;
                lastPosition = e.GetCurrentPoint(CurveCanvas).Position;
                handle.CapturePointer(e.Pointer);
            }
        };

        handle.PointerMoved += (s, e) =>
        {
            if (isDragging)
            {
                var currentPosition = e.GetCurrentPoint(CurveCanvas).Position;
                var delta = new Point(currentPosition.X - lastPosition.X, currentPosition.Y - lastPosition.Y);

                var newPosition = new Point(Canvas.GetLeft(handle) + delta.X + handle.Width / 2, Canvas.GetTop(handle) + delta.Y + handle.Height / 2);

                // Clip the newPosition to the Canvas bounds
                newPosition.X = Math.Clamp(newPosition.X, 0, CurveCanvas.Width);
                newPosition.Y = Math.Clamp(newPosition.Y, 0, CurveCanvas.Height);

                onDrag(newPosition);

                lastPosition = currentPosition;
            }
        };

        handle.PointerReleased += (s, e) =>
        {
            isDragging = false;
            handle.ReleasePointerCapture(e.Pointer);
        };

        handle.PointerCanceled += (s, e) =>
        {
            isDragging = false;
            handle.ReleasePointerCapture(e.Pointer);
        };
    }
}
