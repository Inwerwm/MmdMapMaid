using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;

namespace MmdMapMaid.ViewModels;

public partial class ExtractEmdViewModel : ObservableRecipient
{
    internal EmdExtractorState Extractor
    {
        get;
    }

    [ObservableProperty]
    private string _progressTitle;
    [ObservableProperty]
    private bool _openProgressInfobar;

    public ExtractEmdViewModel()
    {
        Extractor = App.GetService<EmdExtractorState>();
        
        _progressTitle = "";
        _openProgressInfobar = false;
        Extractor.OnCompleted += (_, e) =>
        {
            ProgressTitle = "EmdExtractor_CompletedMessage".GetLocalized();
            OpenProgressInfobar = true;
        };
    }
}
