using MikuMikuMethods.Mme;
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

    private EmmData? Emm
    {
        get;
    }
    private string EmmPath
    {
        get;
    }

    public PmmPathReplacer(string pmmPath)
    {
        Pmm = new(pmmPath);
        PmmPath = pmmPath;

        EmmPath = Path.Combine(Path.GetDirectoryName(PmmPath) ?? ".", Path.GetFileNameWithoutExtension(PmmPath) + ".emm");
        if (File.Exists(EmmPath))
        {
            Emm = new(EmmPath);
        }
    }

    public IEnumerable<(string Name, string Path, int Index)> GetModels() => Pmm.Models.Select((m, i) => (m.Name, m.Path, i));
    public IEnumerable<(string Name, string Path, int Index)> GetAccessories() => Pmm.Accessories.Select((m, i) => (Path.GetFileNameWithoutExtension(m.Name), m.Path, i));

    public void ReplaceModelPath(int targetIndex, string newPath, bool editingEmmTogether)
    {
        var targetPath = Pmm.Models[targetIndex].Path;
        Pmm.Models[targetIndex].Path = newPath;

        if (Emm is not null && editingEmmTogether &&
            Emm.Objects.Find(obj => obj.Path == targetPath) is not null and var targetObject)
        {
            targetObject.Path = newPath;
        }
    }

    public void ReplaceAccessoryPath(int targetIndex, string newPath, bool editingEmmTogether)
    {
        var targetPath = Pmm.Accessories[targetIndex].Path;
        Pmm.Accessories[targetIndex].Path = newPath;

        if (Emm is not null && editingEmmTogether &&
            Emm.Objects.Find(obj => obj.Path == targetPath) is not null and var targetObject)
        {
            targetObject.Path = newPath;
        }
    }

    public string? Save(SaveOptions? options = null)
    {
        options ??= new SaveOptions();

        if(Emm is not null)
        {
            options.SaveWithBackupAndReturnCreatedPath(EmmPath, savePath => Emm.Write(savePath));
        }
        return options.SaveWithBackupAndReturnCreatedPath(PmmPath, savePath => Pmm.Write(savePath));
    }
}
