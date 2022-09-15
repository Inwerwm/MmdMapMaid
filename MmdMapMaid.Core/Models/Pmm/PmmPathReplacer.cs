using MikuMikuMethods.Pmm;

namespace MmdMapMaid.Core.Models.Pmm;
public class PmmPathReplacer
{
    private PolygonMovieMaker Pmm
    {
        get;
    }
    private string PmmPath
    {
        get;
    }

    public PmmPathReplacer(string pmmPath)
    {
        Pmm = new(pmmPath);
        PmmPath = pmmPath;
    }

    public IEnumerable<(string Name, string Path, int Index)> GetModels() => Pmm.Models.Select((m, i) => (m.Name, m.Path, i));
    public IEnumerable<(string Name, string Path, int Index)> GetAccessories() => Pmm.Accessories.Select((m, i) => (m.Name, m.Path, i));

    public void ReplaceModelPath(int targetIndex, string newPath)
    {
        Pmm.Models[targetIndex].Path = newPath;
    }
    public void ReplaceAccessoryPath(int targetIndex, string newPath)
    {
        Pmm.Accessories[targetIndex].Path = newPath;
    }

    public string? Save(SaveOptions? options = null)
    {
        options ??= new SaveOptions();
        return options.SaveWithBackupAndReturnCreatedPath(PmmPath, savePath => Pmm.Write(savePath));
    }
}
