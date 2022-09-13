using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models.Emm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;

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
    private ObservableCollection<IndexedFiledata> _emmModels;
    internal IList<object>? SelectedEmmModels
    {
        get;
        set;
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MapOrderCommand))]
    private bool _isModelSelected;

    public event EventHandler? OnMapStart;
    public event EventHandler? OnMapCompleted;

    private EmmOrderMapper? Mapper
    {
        get;
        set;
    }

    public EmmOrderMapperState()
    {
        _emmModels = new();
    }

    public void UpdateSelectedModels(object _, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        IsModelSelected = SelectedEmmModels?.Any() ?? false;
    }

    [RelayCommand]
    private async Task ReadEmmAsync()
    {
        EmmPath = (await StorageHelper.PickSingleFileAsync(".emm"))?.Path;

        if (EmmPath == null) { return; }

        Mapper = new EmmOrderMapper(EmmPath);

        foreach (var (path, i) in Mapper.ObjectPaths.Select((path, i) => (path, i)).Where(item => Path.GetExtension(item.path).ToLowerInvariant() == ".pmx"))
        {
            EmmModels.Add(new(i, path));
        }
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

        if(SelectedEmmModels is null) { return; }
        foreach (var model in EmmModels.Where(m => m.Path == DestinationPmxPath))
        {
            SelectedEmmModels.Add(model);
        }
    }

    [RelayCommand(CanExecute = nameof(CanMapOrderExecute))]
    private void MapOrder()
    {
        if (SelectedEmmModels is null) { return; }

        OnMapStart?.Invoke(this, EventArgs.Empty);
        Mapper!.Run(SourcePmxPath!, DestinationPmxPath!, SelectedEmmModels.Cast<IndexedFiledata>().Select(m => m.Index));
        OnMapCompleted?.Invoke(this, EventArgs.Empty);
    }

    private bool CanMapOrderExecute() => !(
        Mapper is null ||
        string.IsNullOrWhiteSpace(EmmPath) ||
        string.IsNullOrWhiteSpace(SourcePmxPath) ||
        string.IsNullOrWhiteSpace(DestinationPmxPath)
    ) && IsModelSelected;
}
