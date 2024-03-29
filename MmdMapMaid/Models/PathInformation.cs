﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MmdMapMaid.Models;

public partial class PathInformation : ObservableRecipient
{
    [ObservableProperty]
    private int _index;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _path;
    [ObservableProperty]
    private bool _isEdited;
    [ObservableProperty]
    private bool _isRemoved;
    [ObservableProperty]
    private bool _isRemovable;

    public string InitialPath
    {
        get;
    }

    public PathInformation(int index, string name, string path, bool isRemovable = true)
    {
        _index = index;
        _name = name;
        _path = path;
        _isRemovable = isRemovable;

        InitialPath = path;

        PropertyChanged += (sender, e) =>
        {
            IsEdited = e.PropertyName switch
            {
                nameof(Path) => CompareToInit(),
                nameof(IsRemoved) => CompareToInit(),
                _ => IsEdited
            };
        };
    }

    private bool CompareToInit() => (Path != InitialPath) || IsRemoved;

    [RelayCommand]
    public void RestorePath()
    {
        Path = InitialPath;
        IsRemoved = false;
    }

    [RelayCommand]
    public void Remove()
    {
        IsRemoved = true;
    }
}
