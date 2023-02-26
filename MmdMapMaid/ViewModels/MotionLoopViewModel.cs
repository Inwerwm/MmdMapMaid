using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;

namespace MmdMapMaid.ViewModels;

public class MotionLoopViewModel : ObservableRecipient
{
    internal VmdMotionLoopState MotionLoop
    {
        get;
    }

    public MotionLoopViewModel()
    {
        MotionLoop = App.GetService<VmdMotionLoopState>();
    }
}
