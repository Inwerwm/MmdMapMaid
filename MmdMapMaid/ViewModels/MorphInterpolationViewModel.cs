using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Foundation;

namespace MmdMapMaid.ViewModels;

public partial class MorphInterpolationViewModel : ObservableRecipient
{
    [ObservableProperty]
    private Point _earlierPoint;

    [ObservableProperty]
    private Point _laterPoint;

    [ObservableProperty]
    private int _frameLength;

    [ObservableProperty]
    private double _accuracy;

    [ObservableProperty]
    private string _morphName;

    public MorphInterpolationViewModel()
    {
        EarlierPoint = new(0.25, 0.25);
        LaterPoint = new(0.75, 0.75);
    }
}
