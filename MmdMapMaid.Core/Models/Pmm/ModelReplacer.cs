using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuMethods.Converter;
using MikuMikuMethods.Pmm;
using MikuMikuMethods.Pmx;

namespace MmdMapMaid.Core.Models.Pmm;
public class ModelReplacer
{
    private PolygonMovieMaker Pmm
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

    public void Save(bool enableOverWrite)
    {
        
    }
}
