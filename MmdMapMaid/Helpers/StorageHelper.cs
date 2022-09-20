using Windows.Storage.Pickers;
using Windows.Storage;

namespace MmdMapMaid.Helpers;

class StorageHelper
{
    public static async Task<StorageFile> PickSingleFileAsync(params string[] fileTypes) =>
    await CreateOpenFilePicker(fileTypes).PickSingleFileAsync();

    public static async Task<IEnumerable<StorageFile>> PickMultipleFilesAsync(params string[] fileTypes) =>
        await CreateOpenFilePicker(fileTypes).PickMultipleFilesAsync();

    private static FileOpenPicker CreateOpenFilePicker(params string[] fileTypes)
    {
        var openPicker = new FileOpenPicker();
        foreach (var type in fileTypes)
        {
            openPicker.FileTypeFilter.Add(type);
        }
        RegisterPicker(openPicker);
        return openPicker;
    }

    public static async Task<StorageFolder> PickFolderAsync()
    {
        var folderPicker = new FolderPicker();
        RegisterPicker(folderPicker);
        return await folderPicker.PickSingleFolderAsync();
    }

    private static void RegisterPicker(object picker) => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
}
