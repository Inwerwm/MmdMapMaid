﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Helpers;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class MotionLoopViewModel : ObservableRecipient
{
    internal VmdMotionLoopState MotionLoop
    {
        get;
    }

    public MotionLoopViewModel()
    {
        MotionLoop = App.GetService<VmdMotionLoopState>();
    }

    public void ReadVmd(StorageFile file)
    {
        if(Path.GetExtension(file.Path).ToLower() != ".vmd")
        {
            return;
        }

        MotionLoop.ElementVmdPath = file.Path;
    }

    [RelayCommand]
    private async void ReadVmd()
    {
        var file = await StorageHelper.PickSingleFileAsync(".vmd");
        ReadVmd(file);
    }
}
