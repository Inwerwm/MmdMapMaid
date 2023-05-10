using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
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

    private Point StartPoint
    {
        get;
    }

    private Point EndPoint
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

    private static void EarlierControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);
        UpdateLine(instance.EarlierLine, instance.StartPoint, instance.EarlierControlPoint);
        SetHandlePosition(instance.EarlierControlPointHandle, instance.EarlierControlPoint);
    }

    private static void LaterControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);
        UpdateLine(instance.LaterLine, instance.LaterControlPoint, instance.EndPoint);
        SetHandlePosition(instance.LaterControlPointHandle, instance.LaterControlPoint);
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

        instance.EarlierControlPoint = Mul(instance.EarlierControlPoint, scale);
        instance.LaterControlPoint = Mul(instance.LaterControlPoint, scale);

        instance.DrawCubicBezier(instance.StartPoint, instance.EarlierControlPoint, instance.LaterControlPoint, instance.EndPoint);

        UpdateLine(instance.EarlierLine, instance.StartPoint, instance.EarlierControlPoint);
        UpdateLine(instance.LaterLine, instance.LaterControlPoint, instance.EndPoint);
    }

    private static Point Mul(Point point, double scale) => new(point.X * scale, point.Y * scale);

    private static void UpdateLine(Line line, Point start, Point end)
    {
        line.X1 = start.X;
        line.Y1 = start.Y;
        line.X2 = end.X;
        line.Y2 = end.Y;
    }

    private static void SetHandlePosition(Ellipse handle, Point position)
    {
        Canvas.SetLeft(handle, position.X - handle.Width / 2);
        Canvas.SetTop(handle, position.Y - handle.Height / 2);
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

    private void AddDragHandlers(Ellipse handle, Action<Point> onDrag)
    {
        var isDragging = false;

        handle.PointerPressed += (s, e) =>
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse &&
                e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                isDragging = true;
                handle.CapturePointer(e.Pointer);
            }
        };

        handle.PointerMoved += (s, e) =>
        {
            if (isDragging)
            {
                var currentPosition = e.GetCurrentPoint(CurveCanvas).Position;

                currentPosition.X = Math.Clamp(currentPosition.X, 0, CurveCanvas.Width);
                currentPosition.Y = Math.Clamp(currentPosition.Y, 0, CurveCanvas.Height);

                onDrag(currentPosition);
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
