using System.Text.RegularExpressions;
using MikuMikuMethods.Mme;
using MikuMikuMethods.Mme.Element;
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

    private EmmObject? FindTargetObject(string path) => Emm?.Objects.Find(obj =>
        Regex.IsMatch(obj.Path, @"^[A-Z]:\\")
            ? (obj.Path == path)
            : Regex.IsMatch(path, Regex.Escape(obj.Path) + "$"));

    private void Replace(Func<string> getPath, Action<string> setPath, string newPath, bool editingEmmTogether)
    {
        var targetPath = getPath();
        setPath(newPath);

        if (Emm is not null && editingEmmTogether &&
            FindTargetObject(targetPath) is not null and var targetObject)
        {
            targetObject.Path = newPath;
        }
    }

    public void ReplaceModelPath(int targetIndex, string newPath, bool editingEmmTogether)
    {
        Replace(
            () => Pmm.Models[targetIndex].Path,
            path => Pmm.Models[targetIndex].Path = path,
            newPath,
            editingEmmTogether);
    }

    public void ReplaceAccessoryPath(int targetIndex, string newPath, bool editingEmmTogether)
    {
        Replace(
            () => Pmm.Accessories[targetIndex].Path,
            path => Pmm.Accessories[targetIndex].Path = path,
            newPath,
            editingEmmTogether);
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
