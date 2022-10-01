using MikuMikuMethods.Converter;
using MikuMikuMethods.Pmm;

namespace MmdMapMaid.Core.Models.Pmm;
public class VmdExtractor
{
    public PolygonMovieMaker Pmm
    {
        get;
    }

    private string PmmPath
    {
        get;
    }

    public string[] ModelNames => Pmm.Models.Select(m => m.Name).ToArray();

    public VmdExtractor(string pmmPath)
    {
        PmmPath = pmmPath;
        Pmm = new(PmmPath);
    }

    public void ExtractModelMotion(int modelIndex, string saveDirectory, ModelMotionExtractionOptions? options = null)
    {
        var targetModel = Pmm.Models[modelIndex];
        targetModel.ExtractModelMotion(options).Write(Path.Combine(saveDirectory, $"{Path.GetFileNameWithoutExtension(PmmPath)}_{targetModel.Name}.vmd"));
    }

    public void ExtractCameraMotion(string saveDirectory, CameraMotionExtractionOptions? options = null)
    {
        Pmm.ExtractCameraMotion(options).Write(Path.Combine(saveDirectory, $"{Path.GetFileNameWithoutExtension(PmmPath)}_Camera.vmd"));
    }
}
