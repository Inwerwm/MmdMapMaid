using MikuMikuMethods.Mme;
using MikuMikuMethods.Mme.Element;
using MikuMikuMethods.Pmx;

namespace MmdMapMaid.Core.Models.Emm;
public class EmmOrderMapper
{
    public static IEnumerable<string> GetObjectPaths(string emmPath) => new EmmData(emmPath).Objects.Select(obj => obj.Path);

    public EmmSaveOptions Options
    {
        get;
        init;
    }

    public EmmOrderMapper(EmmSaveOptions? options = null)
    {
        Options = options ?? new();
    }

    /// <summary>
    /// EMMファイルの材質順をモデルに合わせて並び替える
    /// </summary>
    /// <param name="sourceModelPath">並替前材質順のモデル</param>
    /// <param name="destinationModelPath">並替先材質順のモデル</param>
    /// <param name="targetEmmPath">変換対象EMMファイル</param>
    /// <returns>新規作成ファイルのパス(上書き時はバックアップ、そうでなければ並び替え後ファイル)</returns>
    public string? Run(string sourceModelPath, string destinationModelPath, string targetEmmPath)
    {
        var mappedEmm = MapOrder(Path.GetFullPath(sourceModelPath), Path.GetFullPath(destinationModelPath), targetEmmPath);

        var otherDir = Options.GenerationDirectory ?? Path.GetDirectoryName(targetEmmPath) ?? "";
        var otherPath = Path.Combine(otherDir, $"{Path.GetFileNameWithoutExtension(targetEmmPath)}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.emm");
        var (savePath, backupPath) = Options.EnableOverwrite ? (targetEmmPath, otherPath) : (otherPath, targetEmmPath);

        if (Options.EnableOverwrite && Options.CreateBackupIfOverwrite)
        {
            File.Copy(targetEmmPath, backupPath);
        }
        mappedEmm.Write(savePath);
        return !Options.EnableOverwrite || Options.CreateBackupIfOverwrite ? otherPath : null;
    }

    private static EmmData MapOrder(string sourceModelPath, string destinationModelPath, string targetEmmPath)
    {
        var sourceModel = new PmxModel(sourceModelPath);
        var destinationModel = new PmxModel(destinationModelPath);

        var indexMap = sourceModel.Materials.Select(srcMat => destinationModel.Materials.FindIndex(destMat => srcMat.Name == destMat.Name)).ToArray();

        var emm = new EmmData(targetEmmPath);
        var targetObjects = emm.Objects.FindAll(obj => obj.Path == sourceModelPath);

        foreach (var targetObj in targetObjects)
        {
            targetObj.Path = destinationModelPath;
        }

        foreach (var objSetting in emm.EffectSettings.SelectMany(es => es.ObjectSettings).Where(os => targetObjects.Contains(os.Object)))
        {
            var mappedSubset = new EmmMaterial[destinationModel.Materials.Count];

            foreach (var (subset, i) in objSetting.Subsets.Select((s, i) => (s, i)))
            {
                var mappedIndex = indexMap[i];
                if (mappedIndex == -1) { continue; }

                mappedSubset[mappedIndex] = subset;
            }

            objSetting.Subsets.Clear();
            foreach (var subset in mappedSubset)
            {
                objSetting.Subsets.Add(subset ?? new());
            }
        }

        return emm;
    }
}
