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
    private string _vmdWriteInfobarMessage;

    [ObservableProperty]
    private bool _openCompleteMessage;

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
        _vmdWriteInfobarMessage = "Message_VmdWriteComplete".GetLocalized();
    }

    internal void ReadVmd(StorageFile file) => ReadVmd(file.Path);

    private async Task OpenCompleteMessageAutoClose()
    {
        var src = new CancellationTokenSource();
        void onOpenCompleteMessageChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OpenCompleteMessage))
            {
                src.Cancel();
            }
        }

        try
        {
            OpenCompleteMessage = true;
            PropertyChanged += onOpenCompleteMessageChanged;

            await Task.Delay(1000, src.Token);
        }
        finally
        {
            PropertyChanged -= onOpenCompleteMessageChanged;
        }

        if (!src.IsCancellationRequested)
        {
            OpenCompleteMessage = false;
        }
    }


    [RelayCommand]
    private async Task ReadVmd()
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
    public async Task ReadGuideVmd()
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
        if (string.IsNullOrWhiteSpace(VmdPath)) { return; }

        var vmd = new VocaloidMotionData(VmdPath);
        var modelName = vmd.ModelName;

        if (EnableOffsetScaling)
        {
            ApplyFrameModification(vmd, frames => VmdRangeEditor.ScaleOffset(frames, OffsetScale));
        }

        if (EnableGenerateAlignedFrames && string.IsNullOrWhiteSpace(GuideVmdPath))
        {
            ApplyFrameModification(vmd, frames => VmdRangeEditor.GenerateAlignedFrames(frames, new VocaloidMotionData(GuideVmdPath)));
        }

        vmd.ModelName = modelName;

        var saveOperations = new Core.Models.SaveOperations()
        {
            EnableOverwrite = false
        };
        saveOperations.SaveAndBackupFile(VmdPath, vmd.Write);

        _ = OpenCompleteMessageAutoClose();
    }

    private void ApplyFrameModification(VocaloidMotionData result, Func<VocaloidMotionData, IEnumerable<IVmdFrame>> modificationFunc)
    {
        var modified = modificationFunc(result).ToArray();

        result.Clear();
        result.AddFrames(modified);
    }
}
