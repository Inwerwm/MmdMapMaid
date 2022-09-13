using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;

namespace MmdMapMaid.ViewModels;

public partial class PmmViewModel : ObservableRecipient
{
    [ObservableProperty]
    PmmReplacerState _replacerData;

    public PmmViewModel()
    {
        _replacerData = App.GetService<PmmReplacerState>();
    }
}
