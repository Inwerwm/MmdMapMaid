using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;

namespace MmdMapMaid.ViewModels;

public partial class EmmViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _progressTitle;
    [ObservableProperty]
    private bool _openProgressInfobar;

    internal EmmOrderMapperState OrderMapper
    {
        get;
    }

    public EmmViewModel()
    {
        OrderMapper = App.GetService<EmmOrderMapperState>();
        
        _progressTitle = "";

        OrderMapper.OnMapCompleted += (_, e) =>
        {
            ProgressTitle = "EmmOrderMapper_CompletedMessage".GetLocalized();
            OpenProgressInfobar = true;
        };
    }
}
