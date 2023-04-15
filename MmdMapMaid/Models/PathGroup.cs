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
