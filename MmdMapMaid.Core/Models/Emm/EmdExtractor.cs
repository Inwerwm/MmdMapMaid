using MikuMikuMethods.Mme;

namespace MmdMapMaid.Core.Models.Emm;
public class EmdExtractor
{
    public EmmData Emm
    {
        get;
    }

    private string EmmPath
    {
        get;
    }

    public string[] ObjectPaths => Emm.Objects.Select(obj => obj.Path).ToArray();

    public string[] EffectNames => Emm.EffectSettings.Select(effect => effect.Name).ToArray();

    public EmdExtractor(string emmPath)
    {
        EmmPath = emmPath;
        Emm = new(EmmPath);
    }

    public void Run(int objectIndex, int effectIndex, string saveDirectory)
    {
        var targetObject = Emm.Objects[objectIndex];
        var targetEffect = Emm.EffectSettings[effectIndex];

        var emd = targetEffect.ObjectSettings.FirstOrDefault(s => s.Object == targetObject)?.ToEmd();
        if(emd is null) { return; }

        emd.Write(Path.Combine(saveDirectory, $"{Path.GetFileNameWithoutExtension(targetObject.Name)}_{targetEffect.Name}.emd"));
    }
}
