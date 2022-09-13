using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MmdMapMaid.Observables;

public partial class PmmModelInformation : ObservableRecipient
{
    [ObservableProperty]
    private int _index;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _path;
    [ObservableProperty]
    private bool _isEdited;

    private string InitialPath
    {
        get;
    }

    public PmmModelInformation(int index, string name, string path)
    {
        _index = index;
        _name = name;
        _path = path;

        InitialPath = path;

        PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != nameof(Path)) { return; }

            IsEdited = Path != InitialPath;
        };
    }

    [RelayCommand]
    public void RestorePath()
    {
        Path = InitialPath;
    }
}
