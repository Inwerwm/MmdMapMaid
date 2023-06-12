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

    private string? VmdPath
    {
        get;
        set;
    }

    public VmdRangeEditViewModel()
    {

        EnableOffsetScaling = true;
        OffsetScale = 1.0f;
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

        if (EnableOffsetScaling)
        {
            var frames = vmd.Kind switch
            {
                VmdKind.Camera => VmdRangeEditor.ScaleOffset(vmd.CameraFrames, OffsetScale).Cast<IVmdFrame>(),
                VmdKind.Model => VmdRangeEditor.ScaleOffset(vmd.MotionFrames, OffsetScale).Cast<IVmdFrame>(),
                _ => throw new NotImplementedException(),
            };

            result.AddFrames(frames);
        }

        new Core.Models.SaveOptions().SaveWithBackupAndReturnCreatedPath(VmdPath, vmd.Write);
    }
}
