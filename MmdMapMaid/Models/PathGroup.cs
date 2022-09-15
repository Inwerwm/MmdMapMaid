using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmdMapMaid.Models;
public class PathGroup : List<PathInformation>, IGrouping<string, PathInformation>
{
    public string Key
    {
        get;
    }

    public PathGroup(string key, IEnumerable<PathInformation> items) : base(items)
    {
        Key = key;
    }
}
