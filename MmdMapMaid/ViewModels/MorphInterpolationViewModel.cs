using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Models;
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

    [ObservableProperty]
    private ObservableCollection<PathInformation> _models;

    public MorphInterpolationViewModel()
    {
        EarlierPoint = new(0.25, 0.25);
        LaterPoint = new(0.75, 0.75);

        _models = new();
        Models.Add(makePathInfo("aaa", "aaa/aaa"));
        Models.Add(makePathInfo("bbb", "aaa/bbb"));
    }

    private int itemId;
    private PathInformation makePathInfo(string name, string path)
    {
        var info = new PathInformation(itemId++, name, path);
        info.PropertyChanged += (s, e) =>
        {
            if (((PathInformation)s).IsRemoved)
            {
                Models.Remove(info);
            }
        };

        return info;
    }

    public void UpdateSelectedModel(object _, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        
    }
}
