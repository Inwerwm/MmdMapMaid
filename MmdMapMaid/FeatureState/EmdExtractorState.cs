using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.Core.Models.Emm;
using MmdMapMaid.Helpers;
using MmdMapMaid.Models;

namespace MmdMapMaid.FeatureState;

[INotifyPropertyChanged]
internal partial class EmdExtractorState
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private string? _emmPath;

    [ObservableProperty]
    private ObservableCollection<IndexedFiledata> _emmObjects;
    internal IList<object>? SelectedEmmObjects
    {
        get;
        set;
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _isObjectSelected;


    [ObservableProperty]
    private ObservableCollection<IndexedFiledata> _emmEffects;
    internal IList<object>? SelectedEmmEffects
    {
        get;
        set;
    }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private bool _isEffectSelected;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExtractCommand))]
    private string _saveDir;

    private EmdExtractor? Extractor
    {
        get;
        set;
    }

    public EmdExtractorState()
    {
        _emmObjects = new();
        _emmEffects = new();
        _saveDir = "";
    }

    public void UpdateSelectedObjects(object _, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        IsObjectSelected = SelectedEmmObjects?.Any() ?? false;
    }
    public void UpdateSelectedEffects(object _, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        IsEffectSelected = SelectedEmmEffects?.Any() ?? false;
    }

    public void ReadEmm(string emmPath)
    {
        EmmPath = emmPath;
        Extractor = new(EmmPath);

        foreach (var (path, i) in Extractor.ObjectPaths.Select((path, i) => (path, i)))
        {
            EmmObjects.Add(new(i, path));
        }
        foreach (var (name, i) in Extractor.EffectNames.Select((name, i) => (name, i)))
        {
            EmmEffects.Add(new(i, name));
        }

        SaveDir = Path.GetDirectoryName(EmmPath) ?? "";
    }

    [RelayCommand]
    private async Task ReadEmmAsync()
    {
        var emmPath = (await StorageHelper.PickSingleFileAsync(".emm"))?.Path;

        if (emmPath is null) { return; }

        ReadEmm(emmPath);
    }

    [RelayCommand]
    private async Task ReadSaveFolderAsync()
    {
        var folder = (await StorageHelper.PickFolderAsync())?.Path;

        if (folder is null) { return; }

        SaveDir = folder;
    }

    [RelayCommand(CanExecute = nameof(CanExtractExecute))]
    public void Extract()
    {
        if(Extractor is null) { return; }
        if(SelectedEmmObjects is null) { return; }
        if(SelectedEmmEffects is null) { return; }

        foreach (var obj in SelectedEmmObjects.Cast<IndexedFiledata>().Select(obj => obj.Index))
        {
            foreach (var effect in SelectedEmmEffects.Cast<IndexedFiledata>().Select(effect => effect.Index))
            {
                Extractor.Run(obj, effect, SaveDir);
            }
        }
    }

    private bool CanExtractExecute() => !(
            string.IsNullOrWhiteSpace(EmmPath) ||
            string.IsNullOrWhiteSpace(SaveDir) ||
            Extractor is null
        ) && IsObjectSelected && IsEffectSelected;
}
