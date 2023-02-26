using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public Action<string>? AppendLog
    {
        get;
        set;
    }

    public MotionLoopViewModel()
    {
        MotionLoop = App.GetService<VmdMotionLoopState>();
    }

    public void ReadVmd(StorageFile file)
    {
        if (Path.GetExtension(file.Path).ToLower() != ".vmd")
        {
            return;
        }

        MotionLoop.ElementVmdPath = file.Path;
    }

    [RelayCommand]
    private async void ReadVmd()
    {
        var file = await StorageHelper.PickSingleFileAsync(".vmd");
        if (file is null)
        {
            return;
        }
        ReadVmd(file);
    }

    [RelayCommand]
    private void ExecuteMotionLoop()
    {
        if (string.IsNullOrEmpty(MotionLoop.ElementVmdPath))
        {
            return;
        }

        var log = MotionLoop.Execute();
        AppendLog?.Invoke(log);
    }

    [RelayCommand]
    private void OpenDestinationFolder()
    {
        var destinationFolder = Path.GetDirectoryName(MotionLoop.ElementVmdPath);
        if (string.IsNullOrWhiteSpace(destinationFolder))
        {
            return;
        }

        Process.Start("explorer.exe", destinationFolder);
    }
}
