using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Vmd;
using MmdMapMaid.Helpers;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class VmdRangeEditViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool _enableOffsetScaling;

    [ObservableProperty]
    private bool _enableGenerateAlignedFrames;

    [ObservableProperty]
    private float _offsetScale;

    [ObservableProperty]
    private string _vmdPath;

    [ObservableProperty]
    private string _guideVmdPath;

    [ObservableProperty]
    private int _guideOffset;

    public VmdRangeEditViewModel()
    {
        OffsetScale = 1.0f;
        _vmdPath = "";
        _guideVmdPath = "";
    }

    internal void ReadVmd(StorageFile file) => ReadVmd(file.Path);

    [RelayCommand]
    private async void ReadVmd()
    {
        var file = await StorageHelper.PickSingleFileAsync(".vmd");
        if (file is null)
        {
            return;
        }
        ReadVmd(file.Path);
    }

    private void ReadVmd(string path)
    {
        VmdPath = path;
    }

    [RelayCommand]
    public async void ReadGuideVmd()
    {
        var file = await StorageHelper.PickSingleFileAsync(".vmd");
        if (file is null)
        {
            return;
        }
        ReadGuideVmd(file.Path);
    }

    public void ReadGuideVmd(string path)
    {
        GuideVmdPath = path;
    }

    [RelayCommand]
    private void WriteVmd()
    {
        if (VmdPath is null) { return; }

        var vmd = new VocaloidMotionData(VmdPath);
        var modelName = vmd.ModelName;

        if (EnableOffsetScaling)
        {
            ApplyFrameModification(vmd, frames => VmdRangeEditor.ScaleOffset(frames, OffsetScale));
        }

        if (EnableGenerateAlignedFrames)
        {
            ApplyFrameModification(vmd, frames => VmdRangeEditor.GenerateAlignedFrames(frames, new VocaloidMotionData(GuideVmdPath)));
        }

        vmd.ModelName = modelName;
        new Core.Models.SaveOptions().SaveWithBackupAndReturnCreatedPath(VmdPath, vmd.Write);
    }

    private void ApplyFrameModification(VocaloidMotionData result, Func<VocaloidMotionData, IEnumerable<IVmdFrame>> modificationFunc)
    {
        var modified = modificationFunc(result).ToArray();

        result.Clear();
        result.AddFrames(modified);
    }

}
