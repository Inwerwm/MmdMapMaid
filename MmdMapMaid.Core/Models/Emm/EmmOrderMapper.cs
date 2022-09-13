using MikuMikuMethods.Mme;
using MikuMikuMethods.Mme.Element;
using MikuMikuMethods.Pmx;

namespace MmdMapMaid.Core.Models.Emm;
public class EmmOrderMapper
{
    public EmmData Emm
    {
        get;
    }

    public EmmOrderMapper(string emmPath)
    {
        Emm = new(emmPath);
    }

    public IEnumerable<string> GetObjectPaths() => Emm.Objects.Select(obj => obj.Path);

    /// <summary>
    /// EMMファイルの材質順をモデルに合わせて並び替える
    /// </summary>
    /// <param name="sourceModelPath">並替前材質順のモデル</param>
    /// <param name="destinationModelPath">並替先材質順のモデル</param>
    /// <param name="targetEmmPath">変換対象EMMファイル</param>
    /// <returns>新規作成ファイルのパス(上書き時はバックアップ、そうでなければ並び替え後ファイル)</returns>
    public string? Run(string sourceModelPath, string destinationModelPath, string targetEmmPath, SaveOptions? options = null)
    {
        MapOrder(Path.GetFullPath(sourceModelPath), Path.GetFullPath(destinationModelPath), targetEmmPath);

        options ??= new SaveOptions();
        return options.SaveWithBackupAndReturnCreatedPath(targetEmmPath, savePath => Emm.Write(savePath));
    }

    private void MapOrder(string sourceModelPath, string destinationModelPath, string targetEmmPath)
    {
        var sourceModel = new PmxModel(sourceModelPath);
        var destinationModel = new PmxModel(destinationModelPath);

        var indexMap = sourceModel.Materials.Select(srcMat => destinationModel.Materials.FindIndex(destMat => srcMat.Name == destMat.Name)).ToArray();

        var targetObjects = Emm.Objects.FindAll(obj => obj.Path == sourceModelPath);

        foreach (var targetObj in targetObjects)
        {
            targetObj.Path = destinationModelPath;
        }

        foreach (var objSetting in Emm.EffectSettings.SelectMany(es => es.ObjectSettings).Where(os => targetObjects.Contains(os.Object)))
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
    }
}
