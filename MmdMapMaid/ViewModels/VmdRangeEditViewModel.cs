using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MikuMikuMethods.Vmd;
using MmdMapMaid.Core.Models.Vmd;
using Windows.Storage;

namespace MmdMapMaid.ViewModels;

public partial class VmdRangeEditViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool _enableOffsetScaling;

    [ObservableProperty]
    private float _offsetScale;

    [ObservableProperty]
    private string _vmdPath;

    public VmdRangeEditViewModel()
    {
        OffsetScale = 1.0f;
        _vmdPath = "";
    }

    internal void ReadVmd(StorageFile file) => ReadVmd(file.Path);

    [RelayCommand]
    private void ReadVmd(string path)
    {
        VmdPath = path;
    }

    [RelayCommand]
    private void WriteVmd()
    {
        if(VmdPath is null) { return; }
        if (!EnableOffsetScaling)
        {
            return;
        }

        var vmd = new VocaloidMotionData(VmdPath);
        var result = new VocaloidMotionData()
        {
            ModelName = vmd.ModelName,
        };

        var frames = vmd.Kind switch
        {
            VmdKind.Camera => vmd.CameraFrames.ToList<IVmdFrame>(),
            VmdKind.Model => vmd.MotionFrames.ToList<IVmdFrame>(),
            _ => throw new NotImplementedException(),
        };

        if (EnableOffsetScaling)
        {
            var scaledFrames = VmdRangeEditor.ScaleOffset(frames, OffsetScale);
            result.AddFrames(scaledFrames);
        }

        new Core.Models.SaveOptions().SaveWithBackupAndReturnCreatedPath(VmdPath, vmd.Write);
    }
}
