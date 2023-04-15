using MikuMikuMethods.Vmd;

namespace MmdMapMaid.Core.Models.Vmd;
public class VmdReplacer
{
    VocaloidMotionData Vmd
    {
        get;
    }

    public VmdReplacer(string vmdPath)
    {
        Vmd = new(vmdPath);
    }

    public IEnumerable<string> GetMotions() => Vmd.MotionFrames.Select(frame => frame.Name).Distinct();
    public IEnumerable<string> GetMorphs() => Vmd.MorphFrames.Select(frame => frame.Name).Distinct();

    public void ReplaceMotion(string oldName, string newName)
    {
        Replace(Vmd.MotionFrames, oldName, newName);
    }

    public void ReplaceMorph(string oldName, string newName)
    {
        Replace(Vmd.MorphFrames, oldName, newName);
    }

    private static void Replace<T>(List<T> frames, string oldName, string newName) where T : IVmdFrame
    {
        foreach (var frame in frames.Where(frame => frame.Name == oldName))
        {
            frame.Name = newName;
        }
    }

    public void RemoveMotion(string name)
    {
        Remove(Vmd.MotionFrames, name);
    }

    public void RemoveMoprh(string name)
    {
        Remove(Vmd.MorphFrames, name);
    }

    private static void Remove<T>(List<T> frames, string name) where T : IVmdFrame
    {
        frames.RemoveAll(frame => frame.Name == name);
    }

    public void Save(string savePath) => Vmd.Write(savePath);
}
