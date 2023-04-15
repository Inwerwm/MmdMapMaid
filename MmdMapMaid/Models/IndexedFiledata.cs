namespace MmdMapMaid.Models;
internal record IndexedFiledata(int Index, string Path)
{
    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
}
