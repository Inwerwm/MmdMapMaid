using MikuMikuMethods.Pmm;

namespace MmdMapMaid.Core.Models.Pmm;
public class ModelReplacer
{
    private PolygonMovieMaker Pmm
    {
        get;
    }
    private string PmmPath
    {
        get;
    }

    public ModelReplacer(string pmmPath)
    {
        Pmm = new(pmmPath);
    }

    public IEnumerable<(string Name, string Path, int Index)> GetModelList() => Pmm.Models.Select((m, i) => (m.Name, m.Path, i));

    public void Replace(int oldModelIndex, string newModelPath)
    {
        Pmm.Models[oldModelIndex].Path = newModelPath;
    }

    public string? Save(SaveOptions? options = null)
    {
        options ??= new SaveOptions();
        return options.SaveWithBackupAndReturnCreatedPath(PmmPath, savePath => Pmm.Write(savePath));
    }
}
