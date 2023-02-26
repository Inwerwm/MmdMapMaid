using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;

namespace MmdMapMaid.ViewModels;

public partial class ExtractVmdViewModel : ObservableRecipient
{
    internal VmdExtractorState Extractor
    {
        get;
    }

    [ObservableProperty]
    private string _progressTitle;
    [ObservableProperty]
    private bool _openProgressInfobar;

    public ExtractVmdViewModel()
    {
        Extractor = App.GetService<VmdExtractorState>();

        _progressTitle = "";
        _openProgressInfobar = false;
        Extractor.OnCompleted += (_, e) =>
        {
            ProgressTitle = "Message_VmdWriteComplete".GetLocalized();
            OpenProgressInfobar = true;
        };
    }
}
