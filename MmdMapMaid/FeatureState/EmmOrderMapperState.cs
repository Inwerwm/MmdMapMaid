using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models.Emm;
using MmdMapMaid.Helpers;

namespace MmdMapMaid.FeatureState;

[INotifyPropertyChanged]
internal partial class EmmOrderMapperState
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MapOrderCommand))]
    private string? _emmPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MapOrderCommand))]
    private string? _sourcePmxPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MapOrderCommand))]
    private string? _destinationPmxPath;

    [ObservableProperty]
    private ObservableCollection<string> _emmModelNames;

    private EmmOrderMapper Mapper
    {
        get;
    }

    public EmmOrderMapperState()
    {
        _emmModelNames = new();

        Mapper = new EmmOrderMapper();
    }

    [RelayCommand]
    private async Task ReadEmmAsync()
    {
        EmmPath = (await StorageHelper.PickSingleFileAsync(".emm"))?.Path;
    }

    [RelayCommand]
    private async Task ReadSourcePmx()
    {
        SourcePmxPath = (await StorageHelper.PickSingleFileAsync(".pmx"))?.Path;
    }

    [RelayCommand]
    private async Task ReadDestinationPmx()
    {
        DestinationPmxPath = (await StorageHelper.PickSingleFileAsync(".pmx"))?.Path;
    }

    [RelayCommand(CanExecute = nameof(CanMapOrderExecute))]
    private void MapOrder()
    {
        Mapper.Run(SourcePmxPath!, DestinationPmxPath!, EmmPath!);
    }

    private bool CanMapOrderExecute() => !(
        string.IsNullOrWhiteSpace(EmmPath) ||
        string.IsNullOrWhiteSpace(SourcePmxPath) ||
        string.IsNullOrWhiteSpace(DestinationPmxPath)
    );
}
