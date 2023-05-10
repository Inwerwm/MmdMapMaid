using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace MmdMapMaid.Controls;
public sealed partial class EditableBezierCurve : UserControl
{
    public enum OriginPosition
    {
        TopLeft,
        BottomLeft,
        BottomRight,
        TopRight
    }

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

    public static readonly DependencyProperty OriginPositionProperty =
        DependencyProperty.Register(
            nameof(Origin),
            typeof(OriginPosition),
            typeof(EditableBezierCurve),
            new PropertyMetadata(default(OriginPosition), OriginPositionPropertyChanged));

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

    public OriginPosition Origin
    {
        get => (OriginPosition)GetValue(OriginPositionProperty);
        set => SetValue(OriginPositionProperty, value);
    }

    private Point StartPoint
    {
        get;
        set;
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

        StartPoint = GetStartPoint();
        EndPoint = GetEndPoint();

        // ‰Šú’l‚ð 0.0 ` 1.0 ‚Ì”ÍˆÍ‚ÉÝ’è
        EarlierControlPoint = new Point(0.25, 0.25);
        LaterControlPoint = new Point(0.75, 0.75);

        // •`‰æŽž‚ÉÀ•W‚ðŒvŽZ
        DrawCubicBezier(
            StartPoint,
            CalculateCanvasCoordinates(EarlierControlPoint),
            CalculateCanvasCoordinates(LaterControlPoint),
            EndPoint
        );

        SetHandlePosition(EarlierControlPointHandle, CalculateCanvasCoordinates(EarlierControlPoint));
        SetHandlePosition(LaterControlPointHandle, CalculateCanvasCoordinates(LaterControlPoint));

        AddDragHandlers(EarlierControlPointHandle, p => EarlierControlPoint = CalculateNormalizedCoordinates(p));
        AddDragHandlers(LaterControlPointHandle, p => LaterControlPoint = CalculateNormalizedCoordinates(p));
    }

    private Point GetStartPoint()
    {
        return Origin switch
        {
            OriginPosition.TopLeft => new Point(0, 0),
            OriginPosition.BottomLeft => new Point(0, Size),
            OriginPosition.BottomRight => new Point(Size, Size),
            OriginPosition.TopRight => new Point(Size, 0),
            _ => new Point(0, 0),
        };
    }

    private Point GetEndPoint()
    {
        return Origin switch
        {
            OriginPosition.TopLeft => new Point(Size, Size),
            OriginPosition.BottomLeft => new Point(Size, 0),
            OriginPosition.BottomRight => new Point(0, 0),
            OriginPosition.TopRight => new Point(0, Size),
            _ => new Point(Size, Size),
        };
    }

    private static void EarlierControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(
            instance.StartPoint,
            instance.CalculateCanvasCoordinates(instance.EarlierControlPoint),
            instance.CalculateCanvasCoordinates(instance.LaterControlPoint),
            instance.EndPoint
        );
        UpdateLine(instance.EarlierLine, instance.StartPoint, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
        SetHandlePosition(instance.EarlierControlPointHandle, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
    }

    private static void LaterControlPointPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.DrawCubicBezier(
            instance.StartPoint,
            instance.CalculateCanvasCoordinates(instance.EarlierControlPoint),
            instance.CalculateCanvasCoordinates(instance.LaterControlPoint),
            instance.EndPoint
        );
        UpdateLine(instance.LaterLine, instance.CalculateCanvasCoordinates(instance.LaterControlPoint), instance.EndPoint);
        SetHandlePosition(instance.LaterControlPointHandle, instance.CalculateCanvasCoordinates(instance.LaterControlPoint));
    }

    private static void SizePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        var newSize = (double)dependencyPropertyChangedEventArgs.NewValue;

        instance.CurveCanvas.Width = newSize;
        instance.CurveCanvas.Height = newSize;

        instance.EndPoint = instance.GetEndPoint();

        instance.DrawCubicBezier(
            instance.StartPoint,
            instance.CalculateCanvasCoordinates(instance.EarlierControlPoint),
            instance.CalculateCanvasCoordinates(instance.LaterControlPoint),
            instance.EndPoint
        );

        UpdateLine(instance.EarlierLine, instance.StartPoint, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
        UpdateLine(instance.LaterLine, instance.CalculateCanvasCoordinates(instance.LaterControlPoint), instance.EndPoint);

        SetHandlePosition(instance.EarlierControlPointHandle, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
        SetHandlePosition(instance.LaterControlPointHandle, instance.CalculateCanvasCoordinates(instance.LaterControlPoint));
    }

    private static void OriginPositionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var instance = ((EditableBezierCurve)dependencyObject);
        instance.StartPoint = instance.GetStartPoint();
        instance.EndPoint = instance.GetEndPoint();

        instance.DrawCubicBezier(
            instance.StartPoint,
            instance.CalculateCanvasCoordinates(instance.EarlierControlPoint),
            instance.CalculateCanvasCoordinates(instance.LaterControlPoint),
            instance.EndPoint
        );

        UpdateLine(instance.EarlierLine, instance.StartPoint, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
        UpdateLine(instance.LaterLine, instance.CalculateCanvasCoordinates(instance.LaterControlPoint), instance.EndPoint);

        SetHandlePosition(instance.EarlierControlPointHandle, instance.CalculateCanvasCoordinates(instance.EarlierControlPoint));
        SetHandlePosition(instance.LaterControlPointHandle, instance.CalculateCanvasCoordinates(instance.LaterControlPoint));
    }

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

    private Point CalculateNormalizedCoordinates(Point canvasPoint)
    {
        var x = canvasPoint.X / Size;
        double y = canvasPoint.Y / Size;

        switch (Origin)
        {
            case OriginPosition.BottomLeft:
                y = 1 - y;
                break;
            case OriginPosition.TopRight:
                x = 1 - x;
                break;
            case OriginPosition.BottomRight:
                x = 1 - x;
                y = 1 - y;
                break;
        }

        return new Point(x, y);
    }

    private Point CalculateCanvasCoordinates(Point normalizedPoint)
    {
        var x = normalizedPoint.X * Size;
        var y = normalizedPoint.Y * Size;

        switch (Origin)
        {
            case OriginPosition.BottomLeft:
                y = Size - y;
                break;
            case OriginPosition.TopRight:
                x = Size - x;
                break;
            case OriginPosition.BottomRight:
                x = Size - x;
                y = Size - y;
                break;
        }

        return new Point(x, y);
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
