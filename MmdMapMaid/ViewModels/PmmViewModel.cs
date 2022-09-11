using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MmdMapMaid.Observables;

namespace MmdMapMaid.ViewModels;

public partial class PmmViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<PmmModelInformation> _modelInfo;

    public PmmViewModel()
    {
        _modelInfo = new();
    }
}
