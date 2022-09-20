using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;

namespace MmdMapMaid.ViewModels;

public class ExtractEmdViewModel : ObservableRecipient
{
    internal EmdExtractorState Extractor
    {
        get;
    }

    public ExtractEmdViewModel()
    {
        Extractor = App.GetService<EmdExtractorState>();
    }
}
