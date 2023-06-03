using MmdMapMaid.Models;

namespace MmdMapMaid.Contracts.Services;
public interface IFeatureSettingService
{
    Dictionary<PathInformation, string[]> MorphNames
    {
        get;
    }

    Task InitializeAsync();

    Task SetMorphNamesAsync(Dictionary<PathInformation, string[]> morphNames);
}
