using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;

namespace MmdMapMaid.ViewModels;

public partial class EmmViewModel : ObservableRecipient
{
    internal EmmOrderMapperState OrderMapper
    {
        get;
    }

    public EmmViewModel()
    {
        OrderMapper = App.GetService<EmmOrderMapperState>();
    }
}
