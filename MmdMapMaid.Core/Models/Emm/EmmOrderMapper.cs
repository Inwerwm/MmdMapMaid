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

    private string EmmPath
    {
        get;
    }

    public string[] ObjectPaths
    {
        get;
    }

    public EmmOrderMapper(string emmPath)
    {
        EmmPath = emmPath;
        Emm = new(EmmPath);
        ObjectPaths = Emm.Objects.Select(obj => obj.Path).ToArray();
    }

    /// <summary>
    /// EMMファイルの材質順をモデルに合わせて並び替える
    /// </summary>
    /// <param name="sourceModelPath">並替前材質順のモデル</param>
    /// <param name="destinationModelPath">並替先材質順のモデル</param>
    /// <returns>新規作成ファイルのパス(上書き時はバックアップ、そうでなければ並び替え後ファイル)</returns>
    public string? Run(string sourceModelPath, string destinationModelPath, IEnumerable<int> targetIndices, SaveOptions? options = null)
    {
        MapOrder(Path.GetFullPath(sourceModelPath), Path.GetFullPath(destinationModelPath), targetIndices);

        options ??= new SaveOptions();
        return options.SaveWithBackupAndReturnCreatedPath(EmmPath, savePath => Emm.Write(savePath));
    }

    private void MapOrder(string sourceModelPath, string destinationModelPath, IEnumerable<int> targetIndices)
    {
        var sourceModel = new PmxModel(sourceModelPath);
        var destinationModel = new PmxModel(destinationModelPath);

        var indexMap = sourceModel.Materials.Select(srcMat => destinationModel.Materials.FindIndex(destMat => srcMat.Name == destMat.Name)).ToArray();

        var targetObjects = Emm.Objects.Where((_, i) => targetIndices.Contains(i));

        foreach (var objSetting in Emm.EffectSettings.SelectMany(es => es.ObjectSettings).Where(os => targetObjects.Contains(os.Object)))
        {
            var mappedSubset = new EmmMaterial[destinationModel.Materials.Count];

            foreach (var (subset, i) in objSetting.Subsets.Select((s, i) => (s, i)).TakeWhile((s, i) => i < indexMap.Length))
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
