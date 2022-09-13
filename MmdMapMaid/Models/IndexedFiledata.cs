using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmdMapMaid.Models;
internal record IndexedFiledata(int Index, string Path)
{
    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
}
