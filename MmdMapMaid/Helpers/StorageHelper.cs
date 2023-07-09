using Microsoft.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

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

    public static async Task<StorageFile> PickSaveFileAsync(params KeyValuePair<string, IList<string>>[] fileTypes)
    {
        return await PickSaveFileAsync(null!, fileTypes);
    }

    public static async Task<StorageFile> PickSaveFileAsync(string suggestedFileName, params KeyValuePair<string, IList<string>>[] fileTypes)
    {
        var savePicker = new FileSavePicker();
        if (suggestedFileName != null)
        {
            savePicker.SuggestedFileName = suggestedFileName;
        }

        foreach (var type in fileTypes)
        {
            savePicker.FileTypeChoices.Add(type);
        }
        RegisterPicker(savePicker);
        return await savePicker.PickSaveFileAsync();
    }

    public static async Task<StorageFolder> PickFolderAsync()
    {
        var folderPicker = new FolderPicker();
        RegisterPicker(folderPicker);
        return await folderPicker.PickSingleFolderAsync();
    }

    private static void RegisterPicker(object picker) => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));

    public static void SetAcceptedOperation(DragEventArgs e, DataPackageOperation operation = DataPackageOperation.Copy) =>
        e.AcceptedOperation = operation;

    public static async Task<StorageFile?> ReadDropedFile(DragEventArgs e, string? extension = null)
    {
        var droped = await e.DataView.GetStorageItemsAsync();
        return (extension is null ? droped.FirstOrDefault() : droped.FirstOrDefault(file => (Path.GetExtension(file.Path) ?? "") == extension)) as StorageFile;
    }
}
